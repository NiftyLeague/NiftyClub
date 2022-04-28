using DarkRift;
using NiftyClubPlugins.Common.Domain;

namespace NiftyClubPlugins.Plugins.PlayerSync.Domain
{
	class Player : IDarkRiftSerializable
	{
		public Vector3 Position { get; set; }
		public ushort ID { get; set; }
		public string Nickname { get; set; }
		public string DeviceID { get; }
		public string RoomName { get; protected set; }
		public byte CharacterIndex { get; protected set; }

		public Player ()
		{
			// do nothing
		}

		public Player (Vector3 position, ushort ID, string nickname, string deviceId, string roomName, byte characterIndex)
		{
			Position = position;
			this.ID = ID;
			Nickname = nickname;
			DeviceID = deviceId;
			RoomName = roomName;
			CharacterIndex = characterIndex;
		}

		public void Deserialize (DeserializeEvent e)
		{
			Position = e.Reader.ReadSerializable<Vector3> ();
			ID = e.Reader.ReadUInt16 ();
			Nickname = new string (e.Reader.ReadChars ());
			CharacterIndex = e.Reader.ReadByte ();
		}

		public void Serialize (SerializeEvent e)
		{
			e.Writer.Write (Position);
			e.Writer.Write (ID);
			e.Writer.Write (Nickname);
			e.Writer.Write (CharacterIndex);
		}

		public void UpdatePositionAndRotation (PlayerEntry playerEntry)
		{
			Position.X = playerEntry.PositionX;
			Position.Y = playerEntry.PositionY;
			Position.Z = playerEntry.PositionZ;
		}
	}
}
