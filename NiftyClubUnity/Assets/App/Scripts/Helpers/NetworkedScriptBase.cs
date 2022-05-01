using System.Threading.Tasks;
using DarkRift.Client.Unity;
using UnityEngine;

namespace NiftyClub.Helpers
{
	public class NetworkedScriptBase : MonoBehaviour
	{
		protected UnityClient networkingClient;
		
		#region Unity Methods

		void Awake ()
		{
			AwakeAsync ();
		}

		#endregion

		protected virtual async Task AwakeAsync ()
		{
			networkingClient = FindObjectOfType<UnityClient> ();

			while (networkingClient == null)
			{
				await Task.Yield ();
				
				networkingClient = FindObjectOfType<UnityClient> ();
			}
		}
	}
}