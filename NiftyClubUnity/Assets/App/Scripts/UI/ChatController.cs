using System;
using System.Threading.Tasks;
using DynamicBox.EventManagement;
using NiftyClub.Domain;
using NiftyClub.GameEvents;
using NiftyClub.Helpers;
using NiftyClubPlugins.Common.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace NiftyClub.UI
{
	public class ChatController : NetworkedScriptBase
	{
		[Header ("Info Panel"), SerializeField] private GameObject infoParentPanel;
		
		[Header ("Chat Panel"), SerializeField] private GameObject chatParentPanel;
		[Header ("Chat Panel"), SerializeField] private TMP_InputField chatInputField;

		private bool isInteracted = false;

		private ChatBoxModel _chatBoxModel = new ChatBoxModel ();

		#region Unity Methods

		async void Start ()
		{
			TogglePanels (_chatBoxModel.ChatMode);
			
			await Task.Delay (1000);
			
			EventManager.Instance.Raise (new ChatBoxSetupEvent (_chatBoxModel));
		}
		
		void Update ()
		{
			if (isInteracted)
			{
				isInteracted = false;

				switch (_chatBoxModel.ChatMode)
				{
					case ChatMode.Info:
						_chatBoxModel.ChatMode = ChatMode.Input;

						ActivateInputField ();
						
						break;
					case ChatMode.Input:
						_chatBoxModel.ChatMode = ChatMode.Info;
						
						break;
					default:
						throw new ArgumentOutOfRangeException ();
				}
				
				TogglePanels (_chatBoxModel.ChatMode);
			}
		}

		#endregion

		private async Task ActivateInputField ()
		{
			try
			{
				EventSystem.current.SetSelectedGameObject (null);

				await Task.Yield ();
			
				EventSystem.current.SetSelectedGameObject (chatInputField.gameObject);
			}
			catch (Exception e)
			{
				Console.WriteLine (e);
				throw;
			}
		}
		
		public void OnInteract (InputAction.CallbackContext context)
		{
			if (context.performed)
			{
				isInteracted = true;
			}
		}

		public void OnSubmit (string inputFieldText)
		{
			EventManager.Instance.Raise (new ChatSubmitEvent (networkingClient.ID, inputFieldText));
			// Debug.Log ($"Text: {inputFieldText}");
			
			chatInputField.text = string.Empty;
		}

		private void TogglePanels (ChatMode newChatMode)
		{
			infoParentPanel.SetActive (newChatMode == ChatMode.Info);
			chatParentPanel.SetActive (newChatMode == ChatMode.Input);
		}
	}
}
