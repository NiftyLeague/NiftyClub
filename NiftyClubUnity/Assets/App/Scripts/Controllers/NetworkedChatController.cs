using System;
using DarkRift;
using DarkRift.Client;
using DynamicBox.EventManagement;
using NiftyClub.GameEvents;
using NiftyClub.Helpers;
using NiftyClubPlugins.Common.Enums;
using UnityEngine;

namespace NiftyClub.Controllers
{
	public class NetworkedChatController : NetworkedScriptBase
	{
		#region Unity Methods

		void Start ()
		{
			networkingClient.MessageReceived += OnMessageReceived;
		}
		
		void OnEnable ()
		{
			EventManager.Instance.AddListener<ChatSubmitEvent> (ChatSubmitHandler);
		}

		void OnDisable ()
		{
			EventManager.Instance.RemoveListener<ChatSubmitEvent> (ChatSubmitHandler);
		}

		#endregion

		#region Event Handlers

		private void ChatSubmitHandler (ChatSubmitEvent eventDetails)
		{
			if (networkingClient.ID != eventDetails.ID)
				return;
			
			using (DarkRiftWriter writer = DarkRiftWriter.Create())
			{
				writer.Write (eventDetails.ChatText);

				Message message = Message.Create(Tags.ChatReceived, writer);
				networkingClient.SendMessage(message, SendMode.Unreliable);
			}
		}

		#endregion

		private void OnMessageReceived (object sender, MessageReceivedEventArgs e)
		{
			try
            {
            	using (Message message = e.GetMessage ())
            	{
            		if (message == null)
            			return;
    
            		switch (message.Tag)
            		{
            			case Tags.ChatReceived:
            				using (DarkRiftReader reader = message.GetReader ())
            				{
	                            ushort id = reader.ReadUInt16 ();
	                            string chatText = new string (reader.ReadChars ());
	                            
	                            EventManager.Instance.Raise (new ChatSubmitEvent (id, chatText));
            				}
    
            				break;
            		}
            	}
            }
            catch (Exception exception)
            {
            	Debug.LogError ($"Exception: {exception}");
            	throw;
            }
		}
	}
}