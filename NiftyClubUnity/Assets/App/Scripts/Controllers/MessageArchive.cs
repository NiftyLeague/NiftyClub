using System.Collections.Generic;
using System.Threading.Tasks;
using DarkRift;
using DarkRift.Client;
using NiftyClub.Domain;
using NiftyClub.Helpers;
using NiftyClubPlugins.Common.Enums;
using UnityEngine;

namespace NiftyClub.Controllers
{
	public class MessageArchive : NetworkedScriptBase
	{
		[Header ("Debug"), SerializeField] private bool isDebugOn;
		
		private readonly List<SpawnedPlayerDatum> spawnedPlayerData = new List<SpawnedPlayerDatum> ();

		private bool isSpawnedPlayersRelayed;

		#region Unity Methods

		protected override async Task AwakeAsync ()
		{
			await base.AwakeAsync ();

			if (networkingClient == null)
				return;
			networkingClient.MessageReceived += OnMessageReceived;
		}

		void OnDestroy ()
		{
			if (networkingClient == null)
				return;
			networkingClient.MessageReceived -= OnMessageReceived;
		}

		void Update ()
		{
			if (isSpawnedPlayersRelayed)
				return;

			if (!isSpawnedPlayersRelayed)
			{
				TryRelaySpawnedPlayers ();
			}

			if (!isSpawnedPlayersRelayed)
				return;

			Destroy (gameObject.GetComponent<DontDestroy> ());
			Destroy (gameObject);
		}

		#endregion
		
		private void OnMessageReceived (object sender, MessageReceivedEventArgs e)
		{
			using (Message message = e.GetMessage ())
			{
				if (message == null)
					return;

				if (isDebugOn)
					Debug.Log ($"[OnMessageReceived] {message.Tag}");

				switch (message.Tag)
				{
					case Tags.SpawnPlayer:
						using (DarkRiftReader reader = message.GetReader ())
						{
							Vector3 position = new Vector3 (
								reader.ReadSingle (),
								reader.ReadSingle (),
								reader.ReadSingle ());
							ushort id = reader.ReadUInt16 ();
							string nickname = new string (reader.ReadChars ());
							byte characterIndex = reader.ReadByte ();

							spawnedPlayerData.Add (
								new SpawnedPlayerDatum (
									position,
									id,
									nickname,
									characterIndex));
						}

						break;
				}
			}
		}

		private void TryRelaySpawnedPlayers ()
		{
			PlayerSpawner playerSpawner = FindFirstObjectByType<PlayerSpawner> ();
			if (playerSpawner == null)
				return;

			isSpawnedPlayersRelayed = true;

			foreach (SpawnedPlayerDatum spawnedPlayerDatum in spawnedPlayerData)
			{
				playerSpawner.SpawnPlayer (
					spawnedPlayerDatum.Position,
					spawnedPlayerDatum.ID,
					spawnedPlayerDatum.Nickname,
					spawnedPlayerDatum.CharacterIndex);
			}

			if (isDebugOn)
				Debug.Log ("[TryRelaySpawnedPlayers]");
		}
	}
}
