using DarkRift;
using DarkRift.Server;
using System.Collections.Generic;

namespace NiftyClubPlugins.Plugins.RoomSync.Domain
{
	class Room : IDarkRiftSerializable
	{
		public string Name { get; set; }
		public List<IClient> Clients = new List<IClient> ();

		public Room ()
		{
			// do nothing
		}

		public Room (string name, IClient client)
		{
			Name = name;
			Clients.Add (client);
		}

		public void Clone (Room room)
		{
			Name = Name;
			Clients = new List<IClient> (room.Clients);
		}

		public void Deserialize (DeserializeEvent e)
		{
		}

		public void Serialize (SerializeEvent e)
		{
		}
	}
}
