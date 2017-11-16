using Rocket.API;

namespace PhaserArray.VehicleEject
{
	public class VehicleEjectConfiguration : IRocketPluginConfiguration
	{
		public bool LastToLockCanEject;
		public bool DriverCanEject;
		public bool AdminsCanEject;

		public void LoadDefaults()
		{
			LastToLockCanEject = false;
			DriverCanEject = true;
			AdminsCanEject = false;
		}
	}
}
