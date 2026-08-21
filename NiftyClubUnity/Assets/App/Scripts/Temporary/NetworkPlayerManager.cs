using System.Collections.Generic;
using DarkRift.Client.Unity;
using NiftyClub.Helpers;
using UnityEngine;

public class NetworkPlayerManager : NetworkedScriptBase
{
	[Header ("Links"), SerializeField, Tooltip("The DarkRift client to communicate on.")] private UnityClient client;

	public Dictionary<ushort, AgarObject> NetworkPlayers = new Dictionary<ushort, AgarObject>();

	public void AddPlayer (ushort id, AgarObject player)
	{
		NetworkPlayers.Add(id, player);
	}

	public void RemovePlayer (ushort id)
	{
		Destroy (NetworkPlayers[id].gameObject);

		NetworkPlayers.Remove (id);
	}
}
