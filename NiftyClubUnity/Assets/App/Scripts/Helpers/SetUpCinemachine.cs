using System.Threading.Tasks;
using Cinemachine;
using UnityEngine;

namespace NiftyClub.Helpers
{
	public class SetUpCinemachine : MonoBehaviour
	{
		[Header ("Links"), SerializeField] private Transform targetTransform;

		#region Unity Methods

		async void Start ()
		{
			CinemachineVirtualCamera cinemachineVirtualCamera = FindFirstObjectByType<CinemachineVirtualCamera> ();
			while (cinemachineVirtualCamera == null)
			{
				await Task.Yield ();
				
				cinemachineVirtualCamera = FindFirstObjectByType<CinemachineVirtualCamera> ();
			}

			cinemachineVirtualCamera.Follow = targetTransform;
			cinemachineVirtualCamera.LookAt = targetTransform;
		}

		#endregion
	}
}
