using System.Collections.Generic;
using System.Linq;
using DarkRift;
using DarkRift.Client;
using NiftyClub.Helpers;
using NiftyClubPlugins.Common.Enums;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NiftyClub.Controllers
{
	public class LauncherController : NetworkedScriptBase
	{
		[BoxGroup ("Parameters"), SerializeField] private string sceneToLoad;

		[BoxGroup ("Links"), SerializeField] private TMP_InputField usernameInputField;
		[BoxGroup ("Links"), SerializeField] private TMP_Dropdown characterDropdown;
		
		private const string loggedInRoomNamePref = "loggedInRoomName";
		private const string loggedInNicknamePref = "loggedInNickname";
		
		public ConnectionState ConnectionState => networkingClient.ConnectionState;

		private const string DEFAULT_ROOM_NAME = "defaultRoom";

		#region Unity Methods

		protected override void Awake ()
		{
			base.Awake ();

			networkingClient.MessageReceived += OnMessageReceived;
		}

		void Start ()
		{
			PopulateCharacters ();

			if (PlayerPrefs.HasKey (loggedInNicknamePref))
			{
				usernameInputField.text = PlayerPrefs.GetString (loggedInNicknamePref);
			}
		}

		void OnDestroy ()
		{
			networkingClient.MessageReceived -= OnMessageReceived;
		}

		#endregion

		#region Callbacks

		private void OnMessageReceived (object sender, MessageReceivedEventArgs e)
		{
			using (Message message = e.GetMessage ())
			{
				if (message == null)
					return;

				Debug.Log ($"Message Received: {message.Tag}");

				switch (message.Tag)
				{
					case Tags.OnRoomJoined:
						SceneManager.LoadSceneAsync (sceneToLoad, LoadSceneMode.Single);
						
						break;
				}
			}
		}

		#endregion

		public void OnJoinClicked ()
		{
			if (string.IsNullOrEmpty (usernameInputField.text))
				return;
			
			JoinRoom (DEFAULT_ROOM_NAME, usernameInputField.text, (byte) characterDropdown.value);
		}
		
		private void JoinRoom (string roomName, string nickname, byte characterIndex)
		{
			using (DarkRiftWriter writer = DarkRiftWriter.Create ())
			{
				writer.Write (roomName);
				writer.Write (SystemInfo.deviceUniqueIdentifier);
				writer.Write (nickname);
				writer.Write (characterIndex);

				using (Message message = Message.Create (Tags.JoinRoom, writer))
				{
					networkingClient.SendMessage (message, SendMode.Reliable);

					PlayerPrefs.SetString (loggedInRoomNamePref, roomName);
					PlayerPrefs.SetString (loggedInNicknamePref, nickname);
				}
			}
		}

		private List<TMP_Dropdown.OptionData> options;
		
		private void PopulateCharacters ()
		{
			options = new List<TMP_Dropdown.OptionData> ();
			for (int characterIndex = 0; characterIndex < 100; characterIndex++)
			{
				TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData ($"Character #{characterIndex}");
				options.Add (optionData);
			}
			
			characterDropdown.ClearOptions ();
			characterDropdown.AddOptions (options);
		}
	}
}