using System.Threading.Tasks;
using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NiftyClub.Helpers
{
	public class SetUpCinemachine : MonoBehaviour
	{
		[BoxGroup ("Links"), SerializeField] private Transform targetTransform;

		#region Unity Methods

		async void Start ()
		{
			CinemachineVirtualCamera cinemachineVirtualCamera = FindObjectOfType<CinemachineVirtualCamera> ();
			while (cinemachineVirtualCamera == null)
			{
				await Task.Yield ();
				
				cinemachineVirtualCamera = FindObjectOfType<CinemachineVirtualCamera> ();
			}

			cinemachineVirtualCamera.Follow = targetTransform;
			cinemachineVirtualCamera.LookAt = targetTransform;
		}

		#endregion
	}
}