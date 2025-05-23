﻿using System.Diagnostics;
using HabbitStreak.Models;
using HabbitStreak.Services;
using HabbitStreak.States;
using HabbitStreak.ViewModels;

namespace HabbitStreak.Views
{
    public partial class MainPage : ContentPage, IUserDialogService
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainViewModel(this);
        }

        public Task ShowAlertAsync(string title, string message, string cancel)
        {
            return DisplayAlert(title, message, cancel);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is MainViewModel vm)
            {
                await vm.LoadHabbitsAsync();
            }
            mCollectionView.SelectedItem = null;
            //PrintVisualTree(this, 0);
        }

        private void PrintVisualTree(Element element, int indent)
        {
            string prefix = new string(' ', indent * 2);
            string context = element is VisualElement ve && ve.BindingContext != null
                ? $"(BindingContext: {ve.BindingContext.GetType().Name})"
                : "(BindingContext: null)";

            Debug.WriteLine($"{prefix}- {element.GetType().Name} {context}");

            if (element is IElementController controller)
            {
                foreach (var child in controller.LogicalChildren)
                {
                    PrintVisualTree(child, indent + 1);
                }
            }

            // If the element is a ContentView, it might also have Content
            if (element is ContentView contentView && contentView.Content != null)
            {
                PrintVisualTree(contentView.Content, indent + 1);
            }

            // Try to access children for layouts like StackLayout or Grid
            //if (element is Layout layout)
            //{
            //    foreach (var child in layout.Children)
            //    {
            //        PrintVisualTree(child, indent + 1);
            //    }
            //}

            // CollectionView’s generated items are not logical children,
            // so we need to track them separately if necessary
        }

        private async void OnHabbitSelected(object sender, SelectionChangedEventArgs e)
        {
            var collectionView = sender as CollectionView;
            if (e.CurrentSelection != null && e.CurrentSelection.Count > 0)
            {

                var selectedHabbit = e.CurrentSelection[0] as Habbit;
                if (selectedHabbit != null)
                {
                    HabbitState.SelectedHabbit = selectedHabbit;
                    await Shell.Current.GoToAsync(nameof(HabbitDetailPage));
                }
            }
        }
    }
}

