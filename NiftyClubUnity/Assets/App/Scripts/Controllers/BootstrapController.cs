using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace NiftyClub.Controllers
{
	public class BootstrapController : MonoBehaviour
	{
		[Header ("Parameters"), SerializeField] private string launcherScene;

		[Header ("Links"), SerializeField] private TextMeshProUGUI infoText;

		private bool isReadyToLoad = false;

		#region Unity Methods

		async void Start ()
		{
			var sceneLoadAsync = SceneManager.LoadSceneAsync (launcherScene, LoadSceneMode.Single);
			sceneLoadAsync.allowSceneActivation = false;
			SetInfoText ("Loading launcher: 0%");

			while (sceneLoadAsync.progress < 0.9f)
			{
				SetInfoText ($"Loading launcher: {sceneLoadAsync.progress * 100}%");
				
				await Task.Yield ();
			}
			SetInfoText ("Press Enter to transition.");

			while (!isReadyToLoad)
			{
				await Task.Yield ();
			}
			
			sceneLoadAsync.allowSceneActivation = true;
		}

		#endregion

		public void OnInteract (InputAction.CallbackContext context)
		{
			if (context.performed)
			{
				isReadyToLoad = true;
			}
		}

		private void SetInfoText (string text)
		{
			infoText.text = text;
		}
	}
}
