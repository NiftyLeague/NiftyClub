using DarkRift;

namespace NiftyClubPlugins.Common.Domain
{
	public class Quaternion : IDarkRiftSerializable
	{
		public float X { get; set; }
		public float Y { get; set; }
		public float Z { get; set; }
		public float W { get; set; }

		public Quaternion ()
		{

		}

		public Quaternion (float x, float y, float z, float w)
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}

		public void Deserialize (DeserializeEvent e)
		{
			X = e.Reader.ReadSingle ();
			Y = e.Reader.ReadSingle ();
			Z = e.Reader.ReadSingle ();
			W = e.Reader.ReadSingle ();
		}

		public void Serialize (SerializeEvent e)
		{
			e.Writer.Write (X);
			e.Writer.Write (Y);
			e.Writer.Write (Z);
			e.Writer.Write (W);
		}
	}
}
