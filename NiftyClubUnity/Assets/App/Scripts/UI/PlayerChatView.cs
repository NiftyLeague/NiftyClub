using System;
using System.Threading.Tasks;
using DynamicBox.EventManagement;
using NiftyClub.Controllers;
using NiftyClub.GameEvents;
using TMPro;
using UnityEngine;

namespace NiftyClub.UI
{
	public class PlayerChatView : MonoBehaviour
	{
		[Header ("Links"), SerializeField] private NiftyPlayer niftyPlayer;
		
		[Header ("Links"), SerializeField] private GameObject chatParent;
		[Header ("Links"), SerializeField] private TextMeshProUGUI chatText;
		
		#region Unity Methods

		void Start ()
		{
			chatParent.SetActive (false);
		}
		
		private void OnEnable ()
		{
			EventManager.Instance.AddListener<ChatSubmitEvent> (ChatSubmitHandler);
		}

		private void OnDisable ()
		{
			EventManager.Instance.RemoveListener<ChatSubmitEvent> (ChatSubmitHandler);
		}

		#endregion
		
		#region Event Handlers

		private async void ChatSubmitHandler (ChatSubmitEvent eventDetails)
		{
			try
			{
				if (niftyPlayer.ID != eventDetails.ID)
					return;
				
				chatText.text = eventDetails.ChatText;
				chatParent.SetActive (true);

				await Task.Delay (2000);
				
				chatParent.SetActive (false);
			}
			catch (Exception e)
			{
				Console.WriteLine (e);
			}
		}

		#endregion
	}
}
