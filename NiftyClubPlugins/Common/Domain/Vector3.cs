using DarkRift;

namespace NiftyClubPlugins.Common.Domain
{
	class Vector3 : IDarkRiftSerializable
	{
		public float X { get; set; }
		public float Y { get; set; }
		public float Z { get; set; }

		public Vector3 ()
		{
			// do nothing
		}

		public Vector3 (float x, float y, float z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public void Deserialize (DeserializeEvent e)
		{
			X = e.Reader.ReadSingle ();
			Y = e.Reader.ReadSingle ();
			Z = e.Reader.ReadSingle ();
		}

		public void Serialize (SerializeEvent e)
		{
			e.Writer.Write (X);
			e.Writer.Write (Y);
			e.Writer.Write (Z);
		}
	}
}
