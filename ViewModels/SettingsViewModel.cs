namespace HabbitStreak.ViewModels
{
    using System.Windows.Input;
    using HabbitStreak.Models;
    using HabbitStreak.Services;
    using HabbitStreak.Services.HabitStreak.Services;

    public partial class SettingsViewModel : BaseViewModel
    {
        private readonly SettingsService _settingsService;
        private readonly NavigationService _navigationService;
        private SettingsModel _settings;
        public ICommand NavigateBackCommand { get; }

        public bool EnableNotifications
        {
            get => _settings.EnableNotifications;
            set
            {
                if (_settings.EnableNotifications != value)
                {
                    _settings.EnableNotifications = value;
                    OnPropertyChanged(nameof(EnableNotifications));
                    SaveSettingsAsync(); // Save whenever setting changes
                }
            }
        }

        public SettingsViewModel(NavigationService navigationService)
        {
            _navigationService = navigationService;

            NavigateBackCommand = new Command(async () =>
            {
                if (_navigationService != null)
                {
                    await _navigationService.GoBackAsync();
                }
            });

            _settingsService = new SettingsService();
            _settings = new SettingsModel();
            LoadSettingsAsync();
        }

        private async void LoadSettingsAsync()
        {
            _settings = await _settingsService.LoadAsync();
            OnPropertyChanged(nameof(EnableNotifications));
        }

        private async void SaveSettingsAsync()
        {
            await _settingsService.SaveAsync(_settings);
        }
    }
}
