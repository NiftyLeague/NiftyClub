using System.Collections.Generic;
using DarkRift.Client;
using DarkRift.Client.Unity;
using Sirenix.OdinInspector;
using UnityEngine;

public class NetworkPlayerManager : MonoBehaviour
{
	[BoxGroup ("Links"), SerializeField, Tooltip("The DarkRift client to communicate on.")] private UnityClient client;

	public Dictionary<ushort, AgarObject> NetworkPlayers = new Dictionary<ushort, AgarObject>();

	public void Add (ushort id, AgarObject player)
	{
		NetworkPlayers.Add(id, player);
	}

	public void Remove (ushort id)
	{
		Destroy (NetworkPlayers[id].gameObject);

		NetworkPlayers.Remove (id);
	}
}