using DarkRift.Server;
using NiftyClubPlugins.Common.Exceptions;
using NiftyClubPlugins.Plugins.RoomSync;
using NiftyClubPlugins.Plugins.RoomSync.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace NiftyClubPlugins.Common.Utils
{
	class RoomUtils
	{
		public static Room GetRoomByClient (IPluginManager pluginManager, IClient client)
		{
			RoomSyncPlugin roomPlugin = pluginManager.GetPluginByType<RoomSyncPlugin> ();
			Room room = roomPlugin.GetRoomByClient (client);

			if (room == null)
			{
				throw new NoRoomForClientException (client.ID.ToString ());
			}

			return room;
		}

		public static List<IClient> GetClientsInClientRoom (IPluginManager pluginManager, IClient client)
		{
			Room room = GetRoomByClient (pluginManager, client);

			lock (room)
			{
				if (room.Clients == null)
				{
					throw new NoRoomForClientException (client.ID.ToString ());
				}

				List<IClient> clients =
					room.Clients.Count == 0 ?
					new List<IClient> () :
					new List<IClient> (room.Clients);

				return clients;
			}
		}
	}
}
