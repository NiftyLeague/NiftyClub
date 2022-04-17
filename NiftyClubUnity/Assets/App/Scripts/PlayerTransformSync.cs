using AgarPlugin.Domain;
using DarkRift;
using DarkRift.Client;
using DarkRift.Client.Unity;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerTransformSync : MonoBehaviour
{
	[BoxGroup ("Parameters"), SerializeField, Tooltip("The distance we can move before we send a position update.")]
	private float moveDistance = 0.05f;

	public UnityClient Client { get; set; }

	private Transform playerTransform;
	private Vector3 lastPosition;

	#region Unity Methods
		
	void Awake()
	{
		lastPosition = transform.position;
	}

	void Start ()
	{
		playerTransform = transform;
	}

	void Update()
	{
		if (Vector3.Distance(lastPosition, transform.position) > moveDistance)
		{
			Vector3 playerPosition = playerTransform.position;
				
			using (DarkRiftWriter writer = DarkRiftWriter.Create())
			{
				writer.Write(playerPosition.x);
				writer.Write(playerPosition.y);

				using (Message message = Message.Create(Tags.MovePlayerTag, writer))
					Client.SendMessage(message, SendMode.Unreliable);
			}

			lastPosition = playerPosition;
		}
	}

	#endregion
}