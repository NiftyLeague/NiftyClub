using DarkRift;
using UnityEngine;

namespace NiftyClub.Networking.Domain
{
	public class PlayerTransform : IDarkRiftSerializable
	{
		private Vector3 _position;

		public PlayerTransform (Vector3 position)
		{
			_position = position;
		}

		public void Deserialize (DeserializeEvent e)
		{
			_position = new Vector3 (
				e.Reader.ReadSingle (),
				e.Reader.ReadSingle (),
				e.Reader.ReadSingle ());
		}

		public void Serialize (SerializeEvent e)
		{
			e.Writer.Write (_position.x);
			e.Writer.Write (_position.y);
			e.Writer.Write (_position.z);
		}
	}
}