using HabbitStreak.Services.HabitStreak.Services;
using HabbitStreak.ViewModels;

namespace HabbitStreak.Views
{
    public partial class SettingsPage : ContentPage
    {
        private readonly NavigationService _navigationService;
        public SettingsPage(NavigationService navigationService)
        {
            InitializeComponent();
            _navigationService = navigationService;
            BindingContext = new SettingsViewModel(_navigationService);
            Shell.SetBackButtonBehavior(this, new BackButtonBehavior { IsVisible = true });
        }
    }
}