using UnityEngine;

namespace NiftyClub.Domain
{
	public class SpawnedPlayerDatum
	{
		public readonly Vector3 Position;
		public readonly ushort ID;
		public readonly string Nickname;
		public readonly byte CharacterIndex;

		public SpawnedPlayerDatum (Vector3 position, ushort id, string nickname, byte characterIndex)
		{
			Position = position;
			ID = id;
			Nickname = nickname;
			CharacterIndex = characterIndex;
		}
	}
}