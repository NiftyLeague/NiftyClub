using DarkRift;
using DarkRift.Server;
using NiftyClubPlugins.Common.Configs;
using NiftyClubPlugins.Common.Enums;
using NiftyClubPlugins.Common.Exceptions;
using NiftyClubPlugins.Common.Utils;
using NiftyClubPlugins.Plugins.PlayerSync.Domain;
using NiftyClubPlugins.Plugins.RoomSync.Domain;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace NiftyClubPlugins.Plugins.PlayerSync
{
	class PlayerSyncPlugin : Plugin
	{
		public override bool ThreadSafe => true;
		public override Version Version => new Version (0, 1, 1);

		private ConcurrentDictionary<IClient, Player> onlinePlayers = new ConcurrentDictionary<IClient, Player> ();

		private PlayerEntry reusablePlayerEntry = new PlayerEntry ();

		public PlayerSyncPlugin (PluginLoadData pluginLoadData) : base (pluginLoadData)
		{
			ClientManager.ClientConnected += OnClientConnected;
			ClientManager.ClientDisconnected += OnClientDisconnected;
		}

		private void OnClientConnected (object sender, ClientConnectedEventArgs e)
		{
			e.Client.MessageReceived += OnMessageReceived;
		}

		private void OnClientDisconnected (object sender, ClientDisconnectedEventArgs e)
		{
			try
			{
				if (onlinePlayers.ContainsKey (e.Client))
				{
					onlinePlayers.TryRemove (e.Client, out Player value);
				}
			}
			catch (Exception exception)
			{
				Logger.Error ($"Exception: {exception}");
				throw;
			}
		}

		private void OnMessageReceived (object sender, MessageReceivedEventArgs e)
		{
			try
			{
				using (Message message = e.GetMessage ())
				{
					if (message == null)
						return;

					switch (message.Tag)
					{
						case Tags.MovePlayer:
							using (PlayerEntry data = message.Deserialize<PlayerEntry> ())
							{
								// Update the Player instance on the server
								GetPlayer (e.Client).UpdatePositionAndRotation (data);

								data.Id = e.Client.ID;

								List<IClient> clients = RoomUtils.GetClientsInClientRoom (PluginManager, e.Client);

								/* if (CommonConfig.IsDebugOn)
									Logger.Info ($"MovePlayer client: {e.Client.ID}"); */

								message.Serialize (data);

								foreach (IClient sendTo in clients)
								{
									if (sendTo != e.Client)
									{
										sendTo.SendMessage (message, SendMode.Unreliable);
									}
								}
							}

							break;
						case Tags.Jump:
							if (CommonConfig.IsDebugOn)
								Logger.Info ($"AnimationPlugin: {message.Tag}, Client: {e.Client.ID}");

							using (DarkRiftWriter writer = DarkRiftWriter.Create ())
							{
								writer.Write (e.Client.ID);

								message.Serialize (writer);

								List<IClient> clients = RoomUtils.GetClientsInClientRoom (PluginManager, e.Client);

								foreach (IClient client in clients)
								{
									if (client != e.Client)
									{
										client.SendMessage (message, SendMode.Reliable);
									}
								}
							}

							break;
					}
				}
			}
			catch (NoRoomForClientException exception)
			{
				Logger.Info ($"Handled Exception: {exception}");
			}
			catch (Exception exception)
			{
				Logger.Error ($"Exception: {exception}");
				throw;
			}
		}

		public void AddPlayer (IClient client, Player newPlayer)
		{
			onlinePlayers.GetOrAdd (client, newPlayer);
		}

		public Player GetPlayer (IClient client)
		{
			lock (onlinePlayers)
			{
				if (!onlinePlayers.ContainsKey (client))
					return null;

				return onlinePlayers[client];
			}
		}

		public void RemovePlayer (IClient client)
		{
			if (onlinePlayers.ContainsKey (client))
			{
				onlinePlayers.TryRemove (client, out Player value);
			}
		}

		public IEnumerable<Player> GetPlayersInRoom (Room room)
		{
			var playersInRoom = from kvp in onlinePlayers
								where kvp.Value.RoomName == room.Name
								select kvp.Value;

			return playersInRoom;
		}

		public void SendPlayersToNewPlayer (IClient client, Room room)
		{
			try
			{
				var playersInRoom = GetPlayersInRoom (room);

				lock (playersInRoom)
				{
					if (playersInRoom != null && playersInRoom.Any ())
					{
						foreach (var player in playersInRoom)
						{
							if (player.ID != client.ID)
							{
								reusablePlayerEntry.ReadFromPlayer (player);
								using (Message message = Message.Create (Tags.MovePlayer, reusablePlayerEntry))
								{
									client.SendMessage (message, SendMode.Reliable);
								}
							}
						}
					}
				}
			}
			catch (Exception exception)
			{
				Logger.Error ($"Unhandled Exception: {exception}");

				throw;
			}
		}
	}
}
