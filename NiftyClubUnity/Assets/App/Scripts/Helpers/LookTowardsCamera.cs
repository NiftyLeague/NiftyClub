using System;
using UnityEngine;

namespace NiftyClub.Helpers
{
	public class LookTowardsCamera : MonoBehaviour
	{
		private Transform spriteTransform;
		private Transform mainCameraTransform;

		#region Unity Methods

		void Start ()
		{
			spriteTransform = transform;
			mainCameraTransform = Camera.main.transform;
		}

		void LateUpdate ()
		{
			Quaternion lookAngle = Quaternion.LookRotation (mainCameraTransform.forward, Vector3.up);
			spriteTransform.localRotation = Quaternion.Euler (0, lookAngle.eulerAngles.y, 0);
		}

		#endregion
	}
}