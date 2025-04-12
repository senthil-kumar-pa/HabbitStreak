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
        private ObservableCollection<Habbit> _filteredHabbits;
        public ICommand AddHabbitCommand { get; }
        public ICommand MarkCompleteCommand { get; }

        public MainViewModel(IUserDialogService dialogService)
        {
            _dialogService = dialogService;
            _filteredHabbits = Habbits;
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
                    AddHabbit(NewHabbitName, Description, 0);
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

        private string _filterText;
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
        public async Task LoadHabbitsAsync()
        {
            var loaded = await HabbitService.Instance.LoadHabbitsAsync();
            Habbits.Clear();
            foreach (var h in loaded) Habbits.Add(h);

            _filteredHabbits = new ObservableCollection<Habbit>(Habbits);
        }

        private void AddHabbit(string name, string description, int streakCount)
        {
            Habbits.Add(new Habbit(Guid.NewGuid(), name, description, DateTime.Today));
            _ = HabbitService.Instance.SaveHabbitsAsync([.. Habbits]);
        }
    }
}
