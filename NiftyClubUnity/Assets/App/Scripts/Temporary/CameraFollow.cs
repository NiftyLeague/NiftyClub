using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	[Header ("Parameters"), SerializeField] public float speed = 5f;

	private Transform cameraTransform;
	
	public Transform Target { get; set; }

	#region Unity Methods

	void Start ()
	{
		cameraTransform = transform;
	}

	void Update ()
	{
		if (Target != null)
		{
			Vector3 targetPos = Target.GetComponent<Renderer> ().bounds.center;
			cameraTransform.position = Vector3.Lerp (
				transform.position,
				new Vector3 (targetPos.x, targetPos.y, transform.position.z),
				speed * Time.deltaTime
			);
		}
	}

	#endregion
}
