using System.Collections.ObjectModel;
using System.Windows.Input;
using HabbitStreak.Models;
using HabbitStreak.Services;

namespace HabbitStreak.ViewModels
{
    public partial class MainViewModel : BaseViewModel
    {
        //private readonly HabbitService _HabbitService = new();
        public ObservableCollection<Habbit> Habbits { get; } = new();

        public ICommand AddHabbitCommand { get; }
        public ICommand MarkCompleteCommand { get; }

        public MainViewModel()
        {
            AddHabbitCommand = new Command(() =>
            {
                if (!string.IsNullOrWhiteSpace(NewHabbitName))
                    AddHabbit(NewHabbitName, 0);
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

        public async Task LoadHabbitsAsync()
        {
            var loaded = await HabbitService.Instance.LoadHabbitsAsync();
            Habbits.Clear();
            foreach (var h in loaded) Habbits.Add(h);
        }

        private void AddHabbit(string name, int streakCount)
        {
            Habbits.Add(new Habbit(name, DateTime.Today));
            _ = HabbitService.Instance.SaveHabbitsAsync([.. Habbits]);
        }
    }
}
