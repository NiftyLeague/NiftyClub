using DarkRift;
using DarkRift.Server;
using NiftyClubPlugins.Common.Configs;
using NiftyClubPlugins.Common.Enums;
using NiftyClubPlugins.Common.Utils;
using System;
using System.Collections.Generic;

namespace NiftyClubPlugins.Plugins.ChatSync
{
	class ChatSyncPlugin : Plugin
	{
		public override bool ThreadSafe => true;
		public override Version Version => new Version (0, 1, 0);
		public ChatSyncPlugin (PluginLoadData pluginLoadData) : base (pluginLoadData)
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
			// do nothing
		}

		private void OnMessageReceived (object sender, MessageReceivedEventArgs e)
		{
			using (Message message = e.GetMessage ())
			{
				if (message == null)
					return;

				switch (message.Tag)
				{
					case Tags.ChatReceived:
						if (CommonConfig.IsDebugOn)
							Logger.Info ($"ChatSyncPlugin: {message.Tag}, Client: {e.Client.ID}");

						using (DarkRiftReader reader = message.GetReader ())
						{
							string chatText = new string (reader.ReadChars ());

							using (DarkRiftWriter writer = DarkRiftWriter.Create ())
							{
								writer.Write (e.Client.ID);
								writer.Write (chatText);

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
						}

						break;
				}
			}
		}
	}
}
