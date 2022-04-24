using UnityEngine;

namespace NiftyClub.Helpers
{
	public class DontDestroy : MonoBehaviour
	{
		#region Unity Methods

		void Awake ()
		{
			DontDestroyOnLoad (gameObject);
		}

		#endregion
	}
}