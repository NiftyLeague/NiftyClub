using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using NiftyClubPlugins.Common.Enums;

namespace NiftyClub.Domain
{
	public class ChatBoxModel : INotifyPropertyChanged
	{
		private ChatMode _chatMode = ChatMode.Info;
		public ChatMode ChatMode
		{
			get => _chatMode;
			set
			{
				_chatMode = value;
				OnPropertyChanged (nameof (ChatMode));
			}
		}
		
		public event PropertyChangedEventHandler PropertyChanged;
		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged ([CallerMemberName] string propertyName = null)
		{
			if (propertyName == null)
				return;

			PropertyChanged?.Invoke (this, new PropertyChangedEventArgs (propertyName));
		}
	}
}