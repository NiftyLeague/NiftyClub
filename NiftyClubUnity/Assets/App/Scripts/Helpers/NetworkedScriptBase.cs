using System.Threading.Tasks;
using DarkRift.Client.Unity;
using UnityEngine;

namespace NiftyClub.Helpers
{
	public class NetworkedScriptBase : MonoBehaviour
	{
		protected UnityClient networkingClient;
		
		#region Unity Methods

		async void Awake ()
		{
			await AwakeAsync ();
		}

		#endregion

		protected virtual async Task AwakeAsync ()
		{
			networkingClient = FindFirstObjectByType<UnityClient> ();

			while (networkingClient == null)
			{
				await Task.Yield ();
				
				networkingClient = FindFirstObjectByType<UnityClient> ();
			}
		}
	}
}
