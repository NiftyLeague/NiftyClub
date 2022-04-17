using DarkRift;
using UnityEngine;

namespace NiftyClub.Networking.Domain
{
	public class PlayerTransform : IDarkRiftSerializable
	{
		private Vector3 _position;
		private Quaternion _rotation;

		public PlayerTransform (Vector3 position, Quaternion rotation)
		{
			_position = position;
			_rotation = rotation;
		}

		public void Deserialize (DeserializeEvent e)
		{
			_position = new Vector3 (
				e.Reader.ReadSingle (),
				e.Reader.ReadSingle (),
				e.Reader.ReadSingle ());
			_rotation = new Quaternion (
				e.Reader.ReadSingle (),
				e.Reader.ReadSingle (),
				e.Reader.ReadSingle (),
				e.Reader.ReadSingle ());
		}

		public void Serialize (SerializeEvent e)
		{
			e.Writer.Write (_position.x);
			e.Writer.Write (_position.y);
			e.Writer.Write (_position.z);
			
			e.Writer.Write (_rotation.x);
			e.Writer.Write (_rotation.y);
			e.Writer.Write (_rotation.z);
			e.Writer.Write (_rotation.w);
		}
	}
}