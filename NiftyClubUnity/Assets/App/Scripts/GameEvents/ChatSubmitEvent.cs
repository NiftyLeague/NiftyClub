using DynamicBox.EventManagement;

namespace NiftyClub.GameEvents
{
	public class ChatSubmitEvent : GameEvent
	{
		public readonly ushort ID;
		public readonly string ChatText;

		public ChatSubmitEvent (ushort id, string chatText)
		{
			ID = id;
			ChatText = chatText;
		}
	}
}