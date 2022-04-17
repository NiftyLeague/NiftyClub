using NiftyClubPlugins.Common.Domain;

namespace NiftyClubPlugins.Common.Configs
{
	class CommonConfig
	{
		public static readonly Vector3 DefaultSpawnPosition = new Vector3 (0f, 0f, 0f);
		public static readonly Quaternion DefaultSpawnRotation = new Quaternion (0, 0, 0, 0);
		public static readonly Vector3 DefaultLookPosition = new Vector3 (0f, 0f, 0f);

		public static readonly int MaxPlayersInRoom = 250;

		public static readonly bool IsDebugOn = true;
	}
}
