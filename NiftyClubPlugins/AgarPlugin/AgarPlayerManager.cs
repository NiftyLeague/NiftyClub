using AgarPlugin.Domain;
using DarkRift;
using DarkRift.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AgarPlugin
{
    public class AgarPlayerManager : Plugin
    {
        const float MAP_WIDTH = 20;

        public override bool ThreadSafe => false;

        public override Version Version => new Version (1, 0, 3);

        public AgarPlayerManager (PluginLoadData pluginLoadData) : base (pluginLoadData)
        {
            ClientManager.ClientConnected += ClientConnected;
            ClientManager.ClientDisconnected += ClientDisconnected;
        }

		private void ClientDisconnected (object sender, ClientDisconnectedEventArgs e)
		{
            if (players.ContainsKey (e.Client))
			{
                players.Remove (e.Client);

                using (DarkRiftWriter despawnedPlayerWriter = DarkRiftWriter.Create ())
                {
                    despawnedPlayerWriter.Write (e.Client.ID);

                    using (Message newPlayerMessage = Message.Create (Tags.DeSpawnPlayerTag, despawnedPlayerWriter))
                    {
                        foreach (IClient client in ClientManager.GetAllClients ().Where (x => x != e.Client))
                        {
                            client.SendMessage (newPlayerMessage, SendMode.Reliable);
                        }
                    }
                }
            }
        }

        Dictionary<IClient, Player> players = new Dictionary<IClient, Player> ();

        private void ClientConnected (object sender, ClientConnectedEventArgs e)
		{
            Random r = new Random ();
            Player newPlayer = new Player (
                e.Client.ID,
                (float) r.NextDouble () * MAP_WIDTH - MAP_WIDTH / 2,
                (float) r.NextDouble () * MAP_WIDTH - MAP_WIDTH / 2,
                1f,
                (byte) r.Next (0, 200),
                (byte) r.Next (0, 200),
                (byte) r.Next (0, 200)
            );

            using (DarkRiftWriter newPlayerWriter = DarkRiftWriter.Create ())
            {
                ProcessPlayerData (newPlayerWriter, newPlayer);

                using (Message newPlayerMessage = Message.Create (Tags.SpawnPlayerTag, newPlayerWriter))
                {
                    foreach (IClient client in ClientManager.GetAllClients ().Where (x => x != e.Client))
                    {
                        client.SendMessage (newPlayerMessage, SendMode.Reliable);
                    }
                }
            }

            players.Add (e.Client, newPlayer);

            using (DarkRiftWriter playerWriter = DarkRiftWriter.Create ())
            {
                foreach (Player player in players.Values)
                {
                    ProcessPlayerData (playerWriter, player);
                }

                using (Message playerMessage = Message.Create (0, playerWriter))
                {
                    e.Client.SendMessage (playerMessage, SendMode.Reliable);
                }
            }

            e.Client.MessageReceived += MessageReceived;
        }

        private void MessageReceived (object sender, MessageReceivedEventArgs e)
        {
            using (Message message = e.GetMessage () as Message)
            {
                if (message.Tag == Tags.MovePlayerTag)
                {
                    using (DarkRiftReader reader = message.GetReader ())
                    {
                        float newX = reader.ReadSingle ();
                        float newY = reader.ReadSingle ();

                        Player player = players[e.Client];

                        player.X = newX;
                        player.Y = newY;

                        using (DarkRiftWriter writer = DarkRiftWriter.Create ())
                        {
                            writer.Write (player.ID);
                            writer.Write (player.X);
                            writer.Write (player.Y);
                            message.Serialize (writer);
                        }

                        foreach (IClient c in ClientManager.GetAllClients ().Where (x => x != e.Client))
                        {
                            c.SendMessage (message, e.SendMode);
                        }
                    }
                }
            }
        }

        private void ProcessPlayerData (DarkRiftWriter playerWriter, Player player)
		{
            playerWriter.Write (player.ID);
            playerWriter.Write (player.X);
            playerWriter.Write (player.Y);
            playerWriter.Write (player.Radius);
            playerWriter.Write (player.ColorR);
            playerWriter.Write (player.ColorG);
            playerWriter.Write (player.ColorB);
        }
	}
}
