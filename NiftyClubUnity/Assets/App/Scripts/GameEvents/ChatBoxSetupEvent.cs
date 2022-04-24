using DynamicBox.EventManagement;
using NiftyClub.Domain;

namespace NiftyClub.GameEvents
{
	public class ChatBoxSetupEvent : GameEvent
	{
		public readonly ChatBoxModel ChatBox;

		public ChatBoxSetupEvent (ChatBoxModel chatBox)
		{
			ChatBox = chatBox;
		}
	}
}