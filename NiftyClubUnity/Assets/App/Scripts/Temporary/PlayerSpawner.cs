using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DarkRift;
using DarkRift.Client;
using NiftyClub.Controllers;
using NiftyClub.Helpers;
using NiftyClubPlugins.Common.Enums;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerSpawner : NetworkedScriptBase
{
	[BoxGroup ("Links"), SerializeField]
	private Transform parentTransform;

	[BoxGroup ("Prefabs"), SerializeField, Tooltip ("The controllable player prefab.")]
	private NiftyPlayer controllablePrefab;

	[BoxGroup ("Prefabs"), SerializeField, Tooltip ("The network controllable player prefab.")]
	private NiftyPlayer networkPrefab;

	[BoxGroup ("Debug"), SerializeField] private bool isDebugOn;

	private Dictionary<ushort, NiftyPlayer> playerDictionary = new Dictionary<ushort, NiftyPlayer> ();
	public int PlayerCount => playerDictionary.Count;

	public NiftyPlayer LocalPlayer => playerDictionary.FirstOrDefault (player => player.Value.IsLocal).Value;

	#region Unity Methods

	protected override async Task AwakeAsync ()
	{
		await base.AwakeAsync ();
		
		if (networkingClient == null)
		{
			Debug.LogError ("Client unassigned in PlayerSpawner.");
			Application.Quit ();
		}

		if (controllablePrefab == null)
		{
			Debug.LogError ("Controllable Prefab unassigned in PlayerSpawner.");
			Application.Quit ();
		}

		if (networkPrefab == null)
		{
			Debug.LogError ("Network Prefab unassigned in PlayerSpawner.");
			Application.Quit ();
		}

		networkingClient.MessageReceived += OnMessageReceived;
	}

	#endregion

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

							SpawnPlayer (position, id, nickname, characterIndex);
						}

						break;
					case Tags.DespawnPlayer:
						using (DarkRiftReader reader = message.GetReader ())
						{
							ushort id = reader.ReadUInt16 ();

							ThrowIfLocal (id);

							DespawnPlayer (id);
						}

						break;
					case Tags.MovePlayer:
						using (DarkRiftReader reader = message.GetReader ())
						{
							if (reader.Length == 0)
								return;

							Vector3 newPosition = new Vector3 (
								reader.ReadSingle (),
								reader.ReadSingle (),
								reader.ReadSingle ());
							ushort id = reader.ReadUInt16 ();

							ThrowIfLocal (id);

							playerDictionary[id].SetMovePosition (newPosition);
						}

						break;
				}
			}
		}
		catch (Exception exception)
		{
			Debug.LogError ($"Exception: {exception}");
			throw;
		}
	}

	private void OnDisconnected (object sender, DisconnectedEventArgs e)
	{
		DespawnAllPlayers ();
	}

	public void SpawnPlayer (Vector3 position, ushort id, string nickname, byte characterIndex)
	{
		bool isLocal = id == networkingClient.ID;
		NiftyPlayer newPlayer;

		if (isLocal)
		{
			newPlayer = Instantiate (controllablePrefab, parentTransform, false);
			playerDictionary.Add (id, newPlayer);
		}
		else
		{
			newPlayer = Instantiate (networkPrefab, parentTransform, false);
			playerDictionary.Add (id, newPlayer);
		}

		newPlayer.Initialize (position, id, nickname, isLocal, characterIndex);
	}

	private void DespawnPlayer (ushort id)
	{
		Destroy (playerDictionary[id].gameObject);
		playerDictionary.Remove (id);
	}

	private void DespawnAllPlayers ()
	{
		foreach (NiftyPlayer player in playerDictionary.Values)
			Destroy (player.gameObject);

		playerDictionary.Clear ();
	}

	public Dictionary<ushort, NiftyPlayer> GetPlayers ()
	{
		if (isDebugOn)
			Debug.Log ($"[GetPlayers] Characters count: {playerDictionary.Count}");

		return playerDictionary;
	}

	private void ThrowIfLocal (ushort uid)
	{
		if (playerDictionary[uid].IsLocal)
			throw new Exception ("Can't control local player");
	}
}