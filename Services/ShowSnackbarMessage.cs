using CommunityToolkit.Mvvm.Messaging.Messages;

namespace HabbitStreak.Services
{

    public class ShowSnackbarMessage : ValueChangedMessage<string>
    {
        public ShowSnackbarMessage(string value) : base(value) { }
    }

}
