using UnityEngine;

[RequireComponent (typeof (AgarObject))]
public class MouseController : MonoBehaviour
{
	private Camera mainCamera;
	private AgarObject agarObject;

	#region Unity Methods

	void Awake ()
	{
		agarObject = GetComponent<AgarObject> ();
	}

	void Start ()
	{
		mainCamera = Camera.main;
	}

	void Update ()
	{
		Vector3 mousePoint = mainCamera.ScreenToWorldPoint (Input.mousePosition);
		mousePoint.z = 0;

		agarObject.SetMovePosition (mousePoint);
	}

	#endregion
}