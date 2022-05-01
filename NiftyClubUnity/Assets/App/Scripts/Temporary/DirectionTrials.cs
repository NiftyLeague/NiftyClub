using UnityEngine;

namespace NiftyClub.Temporary
{
	public class DirectionTrials : MonoBehaviour
	{
		[SerializeField] private Transform cameraTransform;
		[SerializeField] private Transform playerTransform;
		
		void Update ()
		{
			Debug.Log (cameraTransform.InverseTransformVector (playerTransform.forward));
		}
	}
}