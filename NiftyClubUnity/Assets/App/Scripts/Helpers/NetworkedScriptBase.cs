using DarkRift.Client.Unity;
using UnityEngine;

namespace NiftyClub.Helpers
{
	public class NetworkedScriptBase : MonoBehaviour
	{
		protected UnityClient networkingClient;
		
		#region Unity Methods

		protected virtual void Awake ()
		{
			networkingClient = FindObjectOfType<UnityClient> ();
		}

		#endregion
	}
}