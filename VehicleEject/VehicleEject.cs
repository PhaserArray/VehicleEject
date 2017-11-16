using Rocket.API.Collections;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;

namespace PhaserArray.VehicleEject
{
    public class VehicleEject : RocketPlugin<VehicleEjectConfiguration>
    {
	    public const string Version = "v1.0";
	    public static VehicleEject Instance;
	    public static VehicleEjectConfiguration Config;

	    protected override void Load()
	    {
		    Instance = this;
		    Config = Configuration.Instance;
		    Logger.Log($"Loaded VehicleEject {Version}!");
	    }

	    protected override void Unload()
	    {
		    Logger.Log($"Unloaded VehicleEject {Version}!");
	    }

	    public override TranslationList DefaultTranslations => new TranslationList()
		{
			{"eject_error_notinvehicle", "This command can only be used while in a vehicle!"},
			{"eject_error_notdriver", "This command can only be used by the driver!"},
			{"eject_error_notlasttolock", "This command can only be used by the last person to lock the vehicle!"},
			{"eject_error_noteither", "This command can only be used by the driver or the last person to unlock the vehicle!"},

			{"eject_success_all", "Ejected all passengers!"},
			{"eject_success_allone", "Ejected yourself!"},

			{"eject_success_allother", "Ejected all other passengers!"},
			{"eject_error_noother", "No other passengers found!"},

			{"eject_success_seat", "Ejected {0} from seat {1}!"},
			{"eject_error_notaseat", "{0} is not a valid seat!"},
			{"eject_error_emptyseat", "Seat {0} is empty!"},

			{"eject_success_player", "Ejected {0} from the vehicle!"},
		    {"eject_error_targetnotpassenger", "{0} is not in your vehicle!"},

			{"eject_self", "yourself"},

			{"eject_notice", "You were ejected from the vehicle by {0}!"}
		};
    }
}
