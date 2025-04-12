using HabbitStreak.Services;
using HabbitStreak.Services.HabitStreak.Services;
using HabbitStreak.States;
using HabbitStreak.ViewModels;

namespace HabbitStreak.Views
{
    public partial class HabbitDetailPage : ContentPage, IUserDialogService
    {
        private readonly NavigationService _navigationService;
        public HabbitDetailPage(NavigationService navigationService)
        {
            InitializeComponent();
            _navigationService = navigationService;
            BindingContext = new HabbitDetailViewModel(this, _navigationService);
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