using DarkRift;
using NiftyClubPlugins.Common.Domain;

namespace NiftyClubPlugins.Plugins.PlayerSync.Domain
{
	class Player : IDarkRiftSerializable
	{
		public Vector3 Position { get; set; }
		public Quaternion Rotation { get; set; }
		public ushort ID { get; set; }
		public string Nickname { get; set; }
		public string DeviceID { get; }
		public string RoomName { get; protected set; }

		public Player ()
		{
			// do nothing
		}

		public Player (Vector3 position, Quaternion rotation, ushort ID, string nickname, string deviceId, string roomName)
		{
			Position = position;
			Rotation = rotation;
			this.ID = ID;
			Nickname = nickname;
			DeviceID = deviceId;
			RoomName = roomName;
		}

		public void Deserialize (DeserializeEvent e)
		{
			Position = e.Reader.ReadSerializable<Vector3> ();
			Rotation = e.Reader.ReadSerializable<Quaternion> ();
			ID = e.Reader.ReadUInt16 ();
			Nickname = new string (e.Reader.ReadChars ());
		}

		public void Serialize (SerializeEvent e)
		{
			e.Writer.Write (Position);
			e.Writer.Write (Rotation);
			e.Writer.Write (ID);
			e.Writer.Write (Nickname);
		}

		public void UpdatePositionAndRotation (PlayerEntry playerEntry)
		{
			Position.X = playerEntry.PositionX;
			Position.Y = playerEntry.PositionY;
			Position.Z = playerEntry.PositionZ;

			Rotation.X = playerEntry.RotationX;
			Rotation.Y = playerEntry.RotationY;
			Rotation.Z = playerEntry.RotationZ;
			Rotation.W = playerEntry.RotationW;
		}
	}
}
