using DynamicBox.EventManagement;

namespace NiftyClub.GameEvents
{
	public class ChatSubmitEvent : GameEvent
	{
		public readonly string ChatText;

		public ChatSubmitEvent (string chatText)
		{
			ChatText = chatText;
		}
	}
}