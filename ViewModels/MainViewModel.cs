using System.Collections.ObjectModel;
using System.Windows.Input;
using HabbitStreak.Models;
using HabbitStreak.Services;

namespace HabbitStreak.ViewModels
{
    public partial class MainViewModel : BaseViewModel
    {
        private readonly IUserDialogService _dialogService;
        public ObservableCollection<Habbit> Habbits { get; } = new();
        public ObservableCollection<Habbit> _filteredHabbits;
        public ICommand AddHabbitCommand { get; }
        public ICommand MarkCompleteCommand { get; }

        public MainViewModel(IUserDialogService dialogService)
        {
            _dialogService = dialogService;
            _filteredHabbits = [];

            AddHabbitCommand = new Command(async () =>
            {
                if (!string.IsNullOrWhiteSpace(NewHabbitName))
                {
                    bool exists = await HabbitService.Instance.HabbitExistsAsync(NewHabbitName);
                    if (exists)
                    {
                        await _dialogService.ShowAlertAsync("Duplicate Habbit", $"A habbit named '{NewHabbitName}' already exists.", "OK");
                        return;
                    }

                    FrequencyType frequency;
                    int freqCount = 1;

                    if (IsDaily)
                    {
                        frequency = FrequencyType.Daily;
                    }
                    else if (IsWeekly)
                    {
                        frequency = FrequencyType.Weekly;
                        freqCount = FrequencyCount;
                    }
                    else // Monthly
                    {
                        frequency = FrequencyType.Monthly;
                        freqCount = FrequencyCount;
                    }

                    await AddHabbit(NewHabbitName, Description, frequency, freqCount);
                }
                else
                {
                    await _dialogService.ShowAlertAsync("Error", "Habbit name cannot be empty.", "OK");
                }
            });

            MarkCompleteCommand = new Command<Habbit>(async Habbit =>
            {
                if (Habbit == null)
                    Habbit = Habbits[0];
                Habbit.MarkTodayComplete();
                _ = HabbitService.Instance.SaveHabbitsAsync([.. Habbits]);
                await LoadHabbitsAsync();
            });
        }

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

        public string Description
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

        public ObservableCollection<Habbit> FilteredHabbits
        {
            get => _filteredHabbits;
            set
            {
                _filteredHabbits = value;
                OnPropertyChanged();
            }
        }

        private string _filterText = string.Empty;
        public string FilterText
        {
            get => _filterText;
            set
            {
                _filterText = value;
                OnPropertyChanged();
                ApplyFilter();
            }
        }

        private void ApplyFilter()
        {
            if (string.IsNullOrWhiteSpace(FilterText))
            {
                FilteredHabbits = new ObservableCollection<Habbit>(Habbits);
            }
            else
            {
                FilteredHabbits = new ObservableCollection<Habbit>(
                    Habbits.Where(item => item.Name.ToLower().Contains(FilterText.ToLower())));
            }
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
                FrequencyLabel = "Daily habit";
                IsSliderEnabled = false;
                SliderMaximum = 1;
            }
            else if (IsWeekly)
            {
                FrequencyLabel = $"Times per week: {FrequencyCount}";
                IsSliderEnabled = true;
                SliderMaximum = 7;
            }
            else if (IsMonthly)
            {
                FrequencyLabel = $"Times per month: {FrequencyCount}";
                IsSliderEnabled = true;
                SliderMaximum = 30;
            }
        }

        public async Task LoadHabbitsAsync()
        {
            var loaded = await HabbitService.Instance.LoadHabbitsAsync();
            Habbits.Clear();
            foreach (var h in loaded) Habbits.Add(h);

            FilteredHabbits = new ObservableCollection<Habbit>(Habbits);
        }

        private async Task AddHabbit(string name, string description, FrequencyType frequency, int frequencyCount)
        {
            var habbit = new Habbit(Guid.NewGuid(), name, description, DateTime.Today, null, frequency, frequencyCount);
            Habbits.Add(habbit);
            _ = HabbitService.Instance.SaveHabbitsAsync([.. Habbits]);
            await LoadHabbitsAsync();
        }

    }
}
