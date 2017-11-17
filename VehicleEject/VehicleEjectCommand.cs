using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using Rocket.Core;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Extensions;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace PhaserArray.VehicleEject
{
	public class VehicleEjectCommand : IRocketCommand
	{
		public AllowedCaller AllowedCaller => AllowedCaller.Player;
		public string Name => "eject";
		public string Help => "Allows the driver to eject passengers";
		public string Syntax => "[all | others | steamid | player | seat#]";
		public List<string> Aliases => new List<string>();
		public List<string> Permissions => new List<string> {"vehicle.eject"};

		public void Execute(IRocketPlayer caller, string[] command)
		{
			var player = (UnturnedPlayer)caller;
			if (player.IsInVehicle)
			{
				var vehicle = player.CurrentVehicle;
				if (!HasPermission(caller, "vehicle.canalwayseject") && !(player.IsAdmin && VehicleEject.Config.AdminsCanEject))
				{
					if (!VehicleEject.Config.DriverCanEject &&
						!VehicleEject.Config.LastToLockCanEject)
					{
						Logger.LogWarning("The driver nor the last to lock can eject!");
						return;
					}
					if (VehicleEject.Config.DriverCanEject &&
						!VehicleEject.Config.LastToLockCanEject &&
						!vehicle.checkDriver(player.CSteamID))
					{
						Logger.LogWarning("Caller is not the driver!");
						UnturnedChat.Say(caller, VehicleEject.Instance.Translate("eject_error_notdriver"), Color.red);
						return;
					}
					if (VehicleEject.Config.LastToLockCanEject &&
						!VehicleEject.Config.DriverCanEject &&
						vehicle.lockedOwner != player.CSteamID)
					{
						Logger.LogWarning("Caller is not the last to lock!");
						UnturnedChat.Say(caller, VehicleEject.Instance.Translate("eject_error_notlasttolock"), Color.red);
						return;
					}
					if (VehicleEject.Config.LastToLockCanEject &&
						VehicleEject.Config.DriverCanEject &&
						vehicle.lockedOwner != player.CSteamID &&
						!vehicle.checkDriver(player.CSteamID))
					{
						Logger.LogWarning("Caller is not the driver nor the last to lock!");
						UnturnedChat.Say(caller, VehicleEject.Instance.Translate("eject_error_noteither"), Color.red);
						return;
					}
				}

				if (command.Length == 1)
				{
					var arg = command[0].ToLower();
					if (arg == "all" || arg == "a")
					{
						var playersEjected = EjectAllFromVehicle(vehicle);
						if (playersEjected.Count == 1)
						{
							UnturnedChat.Say(caller, VehicleEject.Instance.Translate("eject_success_allone"), Color.green);
						}
						else
						{
							UnturnedChat.Say(caller, VehicleEject.Instance.Translate("eject_success_all"), Color.green);
							SayEjectNotices(playersEjected, player);
						}
						return;
					}
					if (arg == "others" || arg == "other" || arg == "o")
					{
						var playersEjected = EjectAllButFromVehicle(vehicle, player);
						if (playersEjected.Count == 0)
						{
							UnturnedChat.Say(caller, VehicleEject.Instance.Translate("eject_error_noother"), Color.yellow);
						}
						else
						{
							UnturnedChat.Say(caller, VehicleEject.Instance.Translate("eject_success_allother"), Color.green);
							SayEjectNotices(playersEjected, player);
						}
						return;
					}
					if (byte.TryParse(command[0], out var result))
					{
						if (result - 1 >= vehicle.passengers.Length || result == 0)
						{
							UnturnedChat.Say(caller, VehicleEject.Instance.Translate("eject_error_notaseat", result), Color.yellow);
							return;
						}
						if (vehicle.passengers[result - 1].player == null)
						{
							UnturnedChat.Say(caller, VehicleEject.Instance.Translate("eject_error_emptyseat", result), Color.yellow);
							return;
						}
						var name = VehicleEject.Instance.Translate("eject_self");
						if (vehicle.passengers[result - 1].player.ToUnturnedPlayer().CSteamID != player.CSteamID)
						{
							name = vehicle.passengers[result - 1].player.ToUnturnedPlayer().CharacterName;
							SayEjectNotice(vehicle.passengers[result - 1].player.ToUnturnedPlayer(), player);
						}
						UnturnedChat.Say(caller, VehicleEject.Instance.Translate("eject_success_seat", name, result), Color.green);
						EjectSeatFromVehicle(vehicle, (byte)(result - 1));
						return;
					}
					var target = UnturnedPlayer.FromName(arg);
					if (target != null)
					{
						var wasPassenger = EjectPlayerFromVehicle(vehicle, target);
						var name = VehicleEject.Instance.Translate("eject_self");
						if (target.CSteamID != player.CSteamID)
						{
							name = target.CharacterName;
							SayEjectNotice(target, player);
						}
						if (wasPassenger)
						{
							UnturnedChat.Say(caller, VehicleEject.Instance.Translate("eject_success_player", name), Color.green);
						}
						else
						{
							UnturnedChat.Say(caller, VehicleEject.Instance.Translate("eject_error_targetnotpassenger", name), Color.yellow);
						}
						return;
					}
				}
				UnturnedChat.Say(caller, $"Use /{Name} {Syntax}");
			}
			else
			{
				Logger.LogWarning("Player is not in a vehicle!");
				UnturnedChat.Say(caller, VehicleEject.Instance.Translate("eject_error_notinvehicle"), Color.red);
			}
		}

		/// <summary>
		/// Returns whether the player has the requested permission, does not make an exception for admins.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="permission"></param>
		public static bool HasPermission(IRocketPlayer player, string permission)
		{
			return player.GetPermissions().Any(perm => perm.ToString() == permission);
		}

		/// <summary>
		/// Sends eject notice to ejected player. Does not send notice if ejectee is ejector.
		/// </summary>
		/// <param name="ejectee"></param>
		/// <param name="ejector"></param>
		public static void SayEjectNotice(UnturnedPlayer ejectee, UnturnedPlayer ejector)
		{
			if (ejectee.CSteamID == ejector.CSteamID) return;
			UnturnedChat.Say(ejectee, VehicleEject.Instance.Translate("eject_notice", ejector.CharacterName), Color.green);
		}

		/// <summary>
		/// Sends eject notices to all ejected players.
		/// </summary>
		/// <param name="ejectees"></param>
		/// <param name="ejector"></param>
		public static void SayEjectNotices(IEnumerable<UnturnedPlayer> ejectees, UnturnedPlayer ejector)
		{
			foreach (var ejectee in ejectees)
			{
				SayEjectNotice(ejectee, ejector);
			}
		}

		/// <summary>
		/// Kicks a specific player from the vehicle.
		/// </summary>
		/// <param name="vehicle"></param>
		/// <param name="player"></param>
		/// <returns>Whether the player was found in the vehicle.</returns>
		public static bool EjectPlayerFromVehicle(InteractableVehicle vehicle, UnturnedPlayer player)
		{
			for (byte seat = 0; seat < vehicle.passengers.Length; seat++)
			{
				if (vehicle.passengers[seat].player == null || vehicle.passengers[seat].player.ToUnturnedPlayer().CSteamID != player.CSteamID) continue;
				EjectSeatFromVehicle(vehicle, seat);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Ejects all players from the vehicle.
		/// </summary>
		/// <param name="vehicle"></param>
		/// <returns>People kicked from the vehicle.</returns>
		public static List<UnturnedPlayer> EjectAllFromVehicle(InteractableVehicle vehicle)
		{
			var list = new List<UnturnedPlayer>();
			for (byte seat = 0; seat < vehicle.passengers.Length; seat++)
			{
				if (vehicle.passengers[seat].player == null) continue;
				list.Add(vehicle.passengers[seat].player.ToUnturnedPlayer());
				EjectSeatFromVehicle(vehicle, seat);
			}
			return list;
		}

		/// <summary>
		/// Ejects all but the given player from the vehicle.
		/// </summary>
		/// <param name="vehicle"></param>
		/// <param name="exempt"></param>
		/// <returns>People kicked from the vehicle.</returns>
		public static List<UnturnedPlayer> EjectAllButFromVehicle(InteractableVehicle vehicle, UnturnedPlayer exempt)
		{
			var list = new List<UnturnedPlayer>();
			for (byte seat = 0; seat < vehicle.passengers.Length; seat++)
			{
				if (vehicle.passengers[seat].player == null || vehicle.passengers[seat].player.ToUnturnedPlayer().CSteamID == exempt.CSteamID) continue;
				list.Add(vehicle.passengers[seat].player.ToUnturnedPlayer());
				EjectSeatFromVehicle(vehicle, seat);
			}
			return list;
		}

		/// <summary>
		/// Ejects player in a seat from the vehicle.
		/// </summary>
		/// <param name="vehicle"></param>
		/// <param name="seat"></param>
		public static void EjectSeatFromVehicle(InteractableVehicle vehicle, byte seat)
		{
			vehicle.getExit(seat, out var point, out var angle);
			VehicleManager.sendExitVehicle(vehicle, seat, point, angle, false);
		}
	}
}
