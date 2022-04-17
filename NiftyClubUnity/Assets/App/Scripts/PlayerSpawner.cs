using AgarPlugin.Domain;
using DarkRift;
using DarkRift.Client;
using DarkRift.Client.Unity;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
	[BoxGroup ("Links"), SerializeField, Tooltip ("The DarkRift client to communicate on.")]
	private UnityClient client;
	
	[BoxGroup ("Links"), SerializeField, Tooltip("The network player manager.")]
	private NetworkPlayerManager networkPlayerManager;

	[BoxGroup ("Prefabs"), SerializeField, Tooltip ("The controllable player prefab.")]
	private GameObject controllablePrefab;

	[BoxGroup ("Prefabs"), SerializeField, Tooltip ("The network controllable player prefab.")]
	private GameObject networkPrefab;

	#region Unity Methods

	void Awake ()
	{
		if (client == null)
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

		client.MessageReceived += MessageReceived;
	}

	#endregion

	private void MessageReceived(object sender, MessageReceivedEventArgs e)
	{
		using (Message message = e.GetMessage())
		using (DarkRiftReader reader = message.GetReader())
		{
			if (message.Tag == Tags.SpawnPlayerTag)
			{
				if (reader.Length % 17 != 0)
				{
					Debug.LogWarning("Received malformed spawn packet.");
					return;
				}

				while (reader.Position < reader.Length)
				{
					ushort id = reader.ReadUInt16();
					Vector3 position = new Vector3(reader.ReadSingle(), reader.ReadSingle());
					float radius = reader.ReadSingle();
					Color32 color = new Color32(
						reader.ReadByte(), 
						reader.ReadByte(), 
						reader.ReadByte(),
						255
					);
    
					GameObject obj;
					if (id == client.ID)
					{
						obj = Instantiate(controllablePrefab, position, Quaternion.identity) as GameObject;

						PlayerTransformSync player = obj.GetComponent<PlayerTransformSync>();
						player.Client = client;
					}
					else
					{
						obj = Instantiate(networkPrefab, position, Quaternion.identity) as GameObject;
					} 

					AgarObject agarObj = obj.GetComponent<AgarObject>();

					agarObj.SetRadius(radius);
					agarObj.SetColor(color);
					
					networkPlayerManager.Add(id, agarObj);
				}
			}
			else if (message.Tag == Tags.DeSpawnPlayerTag)
			{
				ushort id = reader.ReadUInt16();
				
				if (networkPlayerManager.NetworkPlayers.ContainsKey (id))
				{
					networkPlayerManager.Remove (id);
				}
			}
			else if (message.Tag == Tags.MovePlayerTag)
			{
				ushort id = reader.ReadUInt16();
				Vector3 newPosition = new Vector3(reader.ReadSingle(), reader.ReadSingle(), 0);

				if (networkPlayerManager.NetworkPlayers.ContainsKey (id))
				{
					networkPlayerManager.NetworkPlayers[id].SetMovePosition(newPosition);
				}
			}
		}
	}
}