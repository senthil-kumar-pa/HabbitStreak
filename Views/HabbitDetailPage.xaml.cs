using HabbitStreak.Models;
using HabbitStreak.Services;

namespace HabbitStreak.Views
{
    public partial class HabbitDetailPage : ContentPage
    {
        private Habbit _originalHabbit;
        private string _originalName;

        public HabbitDetailPage(Habbit habbit)
        {
            InitializeComponent();
            _originalHabbit = habbit;
            _originalName = habbit.Name;
            BindingContext = _originalHabbit;
        }

        private async void OnMarkCompletedClicked(object sender, EventArgs e)
        {
            _originalHabbit.MarkTodayComplete();
            await HabbitService.Instance.UpdateHabbitAsync(_originalHabbit);
            await Navigation.PopAsync();
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private void OnEditClicked(object sender, EventArgs e)
        {
            NameEntry.IsReadOnly = false;
            NameEntry.Focus();
            NameEntry.SelectionLength = NameEntry.Text.Length;
            ((Button)sender).IsVisible = false; // Hide Edit button
                                                // Show Save button
            foreach (var child in ((StackLayout)((Button)sender).Parent).Children)
            {
                if (child is Button btn && btn.Text == "Save")
                {
                    btn.IsVisible = true;
                    break;
                }
            }
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            string? newName = NameEntry.Text?.Trim();

            if (string.IsNullOrEmpty(newName))
            {
                await DisplayAlert("Error", "Habbit name cannot be empty.", "OK");
                return;
            }

            if (!string.Equals(newName, _originalName, StringComparison.OrdinalIgnoreCase))
            {
                bool exists = await HabbitService.Instance.HabbitExistsAsync(newName);
                if (exists)
                {
                    await DisplayAlert("Duplicate Habbit", $"A habbit named '{newName}' already exists.", "OK");
                    return;
                }
            }

            await HabbitService.Instance.UpdateHabbitAsync(_originalHabbit, newName);
            await Navigation.PopAsync();
        }
    }
}