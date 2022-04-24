using System;
using System.Threading.Tasks;
using DynamicBox.EventManagement;
using NiftyClub.GameEvents;
using NiftyClubPlugins.Common.Enums;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace NiftyClub.UI
{
	public class ChatController : MonoBehaviour
	{
		[BoxGroup ("Info Panel"), SerializeField] private GameObject infoParentPanel;
		
		[BoxGroup ("Chat Panel"), SerializeField] private GameObject chatParentPanel;
		[BoxGroup ("Chat Panel"), SerializeField] private TMP_InputField chatInputField;

		private bool isInteracted = false;
		private ChatMode chatMode = ChatMode.Info;

		#region Unity Methods

		void Start ()
		{
			TogglePanels (chatMode);
		}
		
		void Update ()
		{
			if (isInteracted)
			{
				isInteracted = false;

				switch (chatMode)
				{
					case ChatMode.Info:
						chatMode = ChatMode.Input;

						ActivateInputField ();
						
						break;
					case ChatMode.Input:
						chatMode = ChatMode.Info;
						
						break;
					default:
						throw new ArgumentOutOfRangeException ();
				}
				
				TogglePanels (chatMode);
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
			EventManager.Instance.Raise (new ChatSubmitEvent (inputFieldText));
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