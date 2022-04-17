using DarkRift;
using DarkRift.Server;
using NiftyClubPlugins.Common.Configs;
using NiftyClubPlugins.Common.Enums;
using NiftyClubPlugins.Plugins.PlayerSync;
using NiftyClubPlugins.Plugins.PlayerSync.Domain;
using NiftyClubPlugins.Plugins.RoomSync.Domain;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace NiftyClubPlugins.Plugins.RoomSync
{
	class RoomSyncPlugin : Plugin
	{
		public override bool ThreadSafe => true;
		public override Version Version => new Version (0, 1, 0);

		private ConcurrentDictionary<string, Room> roomDictionary = new ConcurrentDictionary<string, Room> ();

		public RoomSyncPlugin (PluginLoadData pluginLoadData) : base (pluginLoadData)
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
			Room room = GetRoomByClient (e.Client);

			if (room == null)
				return;

			using (DarkRiftWriter writer = DarkRiftWriter.Create ())
			{
				writer.Write (e.Client.ID);
				using (Message message = Message.Create (Tags.DespawnPlayer, writer))
				{
					room.Clients.Remove (e.Client);

					if (room.Clients.Count > 0)
					{
						List<IClient> clients = new List<IClient> (room.Clients);
						foreach (IClient client in clients)
						{
							if (client != e.Client)
							{
								client.SendMessage (message, SendMode.Reliable);
							}
						}

						if (CommonConfig.IsDebugOn)
							Logger.Info ($"Leaving Room by {e.Client.ID}. Players left in room: {room.Clients.Count}");
					}
					else
					{
						roomDictionary.TryRemove (room.Name, out Room value);

						if (CommonConfig.IsDebugOn)
							Logger.Info ($"Leaving Room by {e.Client.ID}. Players left in room: {room.Clients.Count}. DELETING ROOM");
					}
				}
			}
		}

		private void OnMessageReceived (object sender, MessageReceivedEventArgs e)
		{
			using (Message message = e.GetMessage ())
			{
				if (message == null)
					return;

				PlayerSyncPlugin playerPlugin = PluginManager.GetPluginByType<PlayerSyncPlugin> ();
				switch (message.Tag)
				{
					case Tags.JoinRoom:
						if (CommonConfig.IsDebugOn)
							Logger.Info ($"Joining Room by {e.Client.ID}");

						using (DarkRiftReader reader = message.GetReader ())
						{
							string roomName = new string (reader.ReadChars ());
							string deviceId = new string (reader.ReadChars ());
							string nickname = new string (reader.ReadChars ());

							if (CommonConfig.IsDebugOn)
								Logger.Info ("Room name: " + roomName);

							// Add Player to Room
							Room room;
							bool joinedRoom = true;
							if (roomDictionary.ContainsKey (roomName))
							{
								if (CommonConfig.IsDebugOn)
									Logger.Info (e.Client.ID + " Joining Existing Room");

								room = roomDictionary[roomName];
								lock (room)
								{

									if (room.Clients.Count < CommonConfig.MaxPlayersInRoom)
									{
										room.Clients.Add (e.Client);

										if (CommonConfig.IsDebugOn)
											Logger.Info (e.Client.ID + " Joined Room. Room Clients: " + room.Clients.Count);
									}
									else
									{
										joinedRoom = false;

										if (CommonConfig.IsDebugOn)
											Logger.Info ("Room is full: " + e.Client.ID + " cant join. Room Clients: " + room.Clients.Count);
									}
								}
							}
							else
							{
								if (CommonConfig.IsDebugOn)
									Logger.Info (e.Client.ID + " Creating New Room");

								room = new Room (roomName, e.Client);
								roomDictionary.GetOrAdd (roomName, room);

								if (CommonConfig.IsDebugOn)
									Logger.Info (e.Client.ID + " Created Room");
							}

							lock (room)
							{
								// If Room is full
								if (!joinedRoom)
								{
									using (DarkRiftWriter writer = DarkRiftWriter.Create ())
									{
										using (Message joinedRoomMessage = Message.Create (Tags.RoomFull, writer))
										{
											e.Client.SendMessage (joinedRoomMessage, SendMode.Reliable);
										}
									}

									return;
								}

								// Send OnRoomJoined Event
								using (DarkRiftWriter writer = DarkRiftWriter.Create ())
								{
									using (Message joinedRoomMessage = Message.Create (Tags.OnRoomJoined, writer))
									{
										e.Client.SendMessage (joinedRoomMessage, SendMode.Reliable);
									}
								}

								// Spawn Player
								Player player = new Player (
									CommonConfig.DefaultSpawnPosition,
									CommonConfig.DefaultSpawnRotation,
									e.Client.ID,
									nickname,
									deviceId,
									roomName);

								using (Message spawnMessage = Message.Create (Tags.SpawnPlayer, player))
								{
									foreach (IClient client in room.Clients)
									{
										if (client != e.Client)
											client.SendMessage (spawnMessage, SendMode.Reliable);
									}
								}

								playerPlugin.AddPlayer (e.Client, player);

								foreach (IClient client in room.Clients)
								{
									Player informedPlayer = playerPlugin.GetPlayer (client);

									lock (informedPlayer)
									{
										using (Message spawnMessage = Message.Create (Tags.SpawnPlayer, informedPlayer))
										{
											e.Client.SendMessage (spawnMessage, SendMode.Reliable);
										}
									}
								}

								playerPlugin.SendPlayersToNewPlayer (e.Client, room);
							}
						}
						break;
					case Tags.RoomPlayers:
						using (DarkRiftReader reader = message.GetReader ())
						{
							using (DarkRiftWriter writer = DarkRiftWriter.Create ())
							{
								string roomName = new string (reader.ReadChars ());
								if (roomDictionary.ContainsKey (roomName))
								{
									writer.Write (roomDictionary[roomName].Clients.Count);
								}
								else
								{
									writer.Write (0);
								}
								using (Message roomMessage = Message.Create (Tags.RoomPlayers, writer))
								{
									e.Client.SendMessage (roomMessage, SendMode.Reliable);
								}
							}
						}
						break;
					case Tags.LeaveRoom:
						if (roomDictionary.ContainsKey (GetRoomByClient (e.Client).Name))
						{
							Room room = GetRoomByClient (e.Client);

							if (room.Clients.Count == 1)
							{
								if (CommonConfig.IsDebugOn)
									Logger.Info ("Room Removed");

								roomDictionary.TryRemove (room.Name, out Room value);
							}
							else
							{
								roomDictionary[room.Name].Clients.Remove (e.Client);

								if (CommonConfig.IsDebugOn)
									Logger.Info ("PlayerRemoved from Room");
							}
						}
						playerPlugin.RemovePlayer (e.Client);
						break;
				}
			}
		}

		public Room GetRoomByClient (IClient client)
		{
			Room foundRoom = roomDictionary.Values.FirstOrDefault (room => room.Clients.Contains (client));

			return foundRoom;
		}
	}
}
