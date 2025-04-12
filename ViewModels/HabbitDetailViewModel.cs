using System.Windows.Input;
using HabbitStreak.Models;
using HabbitStreak.Services;
using HabbitStreak.Services.HabitStreak.Services;
using HabbitStreak.States;

namespace HabbitStreak.ViewModels
{
    public class HabbitDetailViewModel : BaseViewModel
    {
        private readonly NavigationService _navigation;
        private readonly IUserDialogService _dialogService;

        public ICommand MarkCompletedCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand BackCommand { get; }

        private string? newHabbitName;
        public string NewHabbitName
        {
            get => newHabbitName ?? "";
            set
            {
                if (newHabbitName != value)
                {
                    newHabbitName = value;
                    OnPropertyChanged();
                }
            }
        }

        private string? newDescription;

        public string NewDescription
        {
            get => newDescription ?? "";
            set
            {
                if (newDescription != value)
                {
                    newDescription = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isReadOnly;
        public bool IsReadOnly
        {
            get => _isReadOnly;
            set => SetProperty(ref _isReadOnly, value);
        }

        private bool _isSaveVisible;
        public bool IsSaveVisible
        {
            get => _isSaveVisible;
            set => SetProperty(ref _isSaveVisible, value);
        }

        private bool _isEditVisible;
        public bool IsEditVisible
        {
            get => _isEditVisible;
            set => SetProperty(ref _isEditVisible, value);
        }

        private Habbit? _currentHabbit;

        public HabbitDetailViewModel(IUserDialogService dialogService, NavigationService navigationService)
        {
            _navigation = navigationService;
            _dialogService = dialogService;
            _currentHabbit = HabbitState.SelectedHabbit!;
            _isReadOnly = true;
            IsReadOnly = true;

            _isSaveVisible = false;
            IsSaveVisible = false;

            _isEditVisible = true;
            IsEditVisible = true;

            NewHabbitName = _currentHabbit?.Name ?? "";
            NewDescription = _currentHabbit?.Description ?? "";

            MarkCompletedCommand = new Command(async () => await MarkCompletedAsync());
            SaveCommand = new Command(async () => await SaveAsync());
            EditCommand = new Command(() => SetEditMode());
            BackCommand = new Command(async () => await _navigation.GoBackAsync());
        }

        private async Task MarkCompletedAsync()
        {
            _currentHabbit!.MarkTodayComplete();
            await HabbitService.Instance.UpdateHabbitAsync(_currentHabbit);
            await _navigation.GoBackAsync();
        }

        private void SetEditMode()
        {
            IsReadOnly = false;
            IsSaveVisible = true;
            IsEditVisible = false;
        }
        private async Task SaveAsync()
        {
            var newName = NewHabbitName.Trim();
            var newDescription = NewDescription?.Trim();

            if (string.IsNullOrEmpty(newName))
            {
                await _dialogService.ShowAlertAsync("Error", "Habbit name cannot be empty.", "OK");
                return;
            }

            if (!string.Equals(newName, _currentHabbit!.Name, StringComparison.OrdinalIgnoreCase))
            {
                bool exists = await HabbitService.Instance.HabbitExistsAsync(newName);
                if (exists)
                {
                    await _dialogService.ShowAlertAsync("Duplicate Habbit", $"A habbit named '{newName}' already exists.", "OK");
                    return;
                }
            }

            await HabbitService.Instance.UpdateHabbitAsync(_currentHabbit!, newName, newDescription ?? "");
            await _navigation.GoBackAsync();
        }
    }
}
