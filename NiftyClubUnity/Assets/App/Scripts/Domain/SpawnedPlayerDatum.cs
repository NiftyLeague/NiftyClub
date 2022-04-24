using UnityEngine;

namespace NiftyClub.Domain
{
	public class SpawnedPlayerDatum
	{
		public readonly Vector3 Position;
		public readonly Quaternion Rotation;
		public readonly ushort ID;
		public readonly string Nickname;

		public SpawnedPlayerDatum (Vector3 position, Quaternion rotation, ushort id, string nickname)
		{
			Position = position;
			Rotation = rotation;
			ID = id;
			Nickname = nickname;
		}
	}
}