using CommunityToolkit.Mvvm.Messaging;
using HabbitStreak.Services;
using HabbitStreak.Services.HabitStreak.Services;
using HabbitStreak.States;
using HabbitStreak.ViewModels;

namespace HabbitStreak.Views
{
    public partial class HabbitDetailPage : ContentPage, IUserDialogService, IRecipient<ShowSnackbarMessage>
    {
        private readonly NavigationService _navigationService;
        public HabbitDetailPage(NavigationService navigationService)
        {
            InitializeComponent();
            _navigationService = navigationService;
            BindingContext = new HabbitDetailViewModel(this, _navigationService);
            WeakReferenceMessenger.Default.Register(this);
        }

        public async void Receive(ShowSnackbarMessage message)
        {
            SnackbarHost.IsVisible = true;

            Snackbar.TranslationX = 0;
            Snackbar.TranslationY = -100;
            Snackbar.Opacity = 0;

            await Task.WhenAll(
                Snackbar.TranslateTo(0, 50, 300, Easing.SinOut),
                Snackbar.FadeTo(1, 300)
            );

            await Task.Delay(2000);

            await Task.WhenAll(
                Snackbar.TranslateTo(0, -50, 300, Easing.SinIn),
                Snackbar.FadeTo(0, 300)
            );

            SnackbarHost.IsVisible = false;
        }

        public Task ShowAlertAsync(string title, string message, string cancel)
        {
            return DisplayAlert(title, message, cancel);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            HabbitState.SelectedHabbit = null;
        }
    }
}