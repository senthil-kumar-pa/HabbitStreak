using System.Collections.ObjectModel;
using System.Windows.Input;
using HabbitStreak.Models;
using HabbitStreak.Resources.AppIcon;
using HabbitStreak.Services;
using HabbitStreak.Services.HabitStreak.Services;
using HabbitStreak.States;

namespace HabbitStreak.ViewModels
{
    public class HabbitDetailViewModel : BaseViewModel
    {
        private readonly NavigationService _navigation;
        private readonly IUserDialogService _dialogService;
        public ObservableCollection<string> IconOptions { get; } = new();
        public ICommand MarkCompletedCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
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

        private string? lastCompleted;
        public string? LastCompleted
        {
            get => lastCompleted ?? "Not completed";
            set
            {
                if (lastCompleted != value)
                {
                    lastCompleted = value;
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

        private FrequencyType newFrequency;

        public FrequencyType NewFrequencyType
        {
            get => newFrequency;
            set
            {
                if (newFrequency != value)
                {
                    newFrequency = value;
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
            IconOptions = new ObservableCollection<string>(IconProvider.HabbitIcons);
            _isReadOnly = true;
            IsReadOnly = true;

            _isSaveVisible = false;
            IsSaveVisible = false;

            _isEditVisible = true;
            IsEditVisible = true;

            NewHabbitName = _currentHabbit?.Name ?? "";
            NewDescription = _currentHabbit?.Description ?? "";
            SelectedIcon = _currentHabbit?.Icon ?? "";
            LastCompleted = _currentHabbit?.LastCompletedDate?.ToString("dd-MMM-yyyy HH:mm:ss") ?? "Not Completed";

            MarkCompletedCommand = new Command(async () => await MarkCompletedAsync());
            SaveCommand = new Command(async () =>
            {
                if (!string.IsNullOrWhiteSpace(NewHabbitName))
                {
                    bool exists = await HabbitService.Instance.HabbitExistsAsync(_currentHabbit!.Id, NewHabbitName);
                    if (exists)
                    {
                        await _dialogService.ShowAlertAsync("Duplicate Habbit", $"A habbit named '{NewHabbitName}' already exists.", "OK");
                        return;
                    }

                    int freqCount = 1;

                    if (IsDaily)
                    {
                        newFrequency = FrequencyType.Daily;
                    }
                    else if (IsWeekly)
                    {
                        newFrequency = FrequencyType.Weekly;
                        freqCount = FrequencyCount;
                    }
                    else // Monthly
                    {
                        newFrequency = FrequencyType.Monthly;
                        freqCount = FrequencyCount;
                    }
                    await SaveAsync();
                }

            });
            CancelCommand = new Command(() => SetEditMode(false));
            EditCommand = new Command(() => SetEditMode(true));
            BackCommand = new Command(async () => await _navigation.GoBackAsync());
        }

        private async Task MarkCompletedAsync()
        {
            _currentHabbit!.MarkTodayComplete();
            await HabbitService.Instance.UpdateHabbitAsync(_currentHabbit);
            await _navigation.GoBackAsync();
        }

        private void SetEditMode(bool editMode)
        {
            IsReadOnly = !editMode;
            IsSaveVisible = editMode;
            IsEditVisible = !editMode;
        }

        private bool isDaily = true;
        public bool IsDaily
        {
            get => isDaily;
            set
            {
                if (SetProperty(ref isDaily, value))
                {
                    OnPropertyChanged(nameof(IsWeeklyOrMonthly));
                    UpdateFrequencyState();
                }
            }
        }

        private bool isWeekly;
        public bool IsWeekly
        {
            get => isWeekly;
            set
            {
                if (SetProperty(ref isWeekly, value))
                {
                    OnPropertyChanged(nameof(IsWeeklyOrMonthly));
                    UpdateFrequencyState();
                }
            }
        }

        private bool isMonthly;
        public bool IsMonthly
        {
            get => isMonthly;
            set
            {
                if (SetProperty(ref isMonthly, value))
                {
                    OnPropertyChanged(nameof(IsWeeklyOrMonthly));
                    UpdateFrequencyState();
                }
            }
        }

        public bool IsWeeklyOrMonthly => IsWeekly || IsMonthly;

        private int frequencyCount = 1;
        public int FrequencyCount
        {
            get => frequencyCount;
            set
            {
                if (SetProperty(ref frequencyCount, value))
                    UpdateFrequencyState();
            }
        }

        private double sliderMaximum = 7;
        public double SliderMaximum
        {
            get => sliderMaximum;
            set => SetProperty(ref sliderMaximum, value);
        }

        private bool isSliderEnabled = true;
        public bool IsSliderEnabled
        {
            get => isSliderEnabled;
            set => SetProperty(ref isSliderEnabled, value);
        }

        private string frequencyLabel = "Times per week: 1";
        public string FrequencyLabel
        {
            get => frequencyLabel;
            set => SetProperty(ref frequencyLabel, value);
        }

        private void UpdateFrequencyState()
        {
            if (IsDaily)
            {
                FrequencyLabel = "Daily";
                IsSliderEnabled = false;
                SliderMaximum = 1;
            }
            else if (IsWeekly)
            {
                FrequencyLabel = $"Times / Week: {FrequencyCount}";
                IsSliderEnabled = true;
                SliderMaximum = 7;
            }
            else if (IsMonthly)
            {
                FrequencyLabel = $"Times / Month: {FrequencyCount}";
                IsSliderEnabled = true;
                SliderMaximum = 30;
            }
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

            await HabbitService.Instance.UpdateHabbitAsync(_currentHabbit!, newName, newDescription ?? "", NewFrequencyType, frequencyCount, SelectedIcon);
            await _navigation.GoBackAsync();
        }

        private string _selectedIcon = string.Empty;
        public string SelectedIcon
        {
            get => _selectedIcon;
            set
            {
                if (_selectedIcon != value)
                {
                    _selectedIcon = value;
                    OnPropertyChanged(nameof(SelectedIcon));
                }
            }
        }
    }
}
