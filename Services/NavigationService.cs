namespace HabbitStreak.Services
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Maui.Controls;

    namespace HabitStreak.Services
    {
        public class NavigationService
        {
            private readonly IServiceProvider _serviceProvider;

            public NavigationService(IServiceProvider serviceProvider)
            {
                _serviceProvider = serviceProvider;
            }

            /// <summary>
            /// Navigates to a page using the route name.
            /// </summary>
            public static async Task NavigateToAsync(string route, object? parameter = null)
            {
                if (parameter != null)
                {
                    var navigationParameter = new Dictionary<string, object>
                {
                    { "Parameter", parameter }
                };

                    await Shell.Current.GoToAsync(route, navigationParameter);
                }
                else
                {
                    await Shell.Current.GoToAsync(route);
                }
            }

            /// <summary>
            /// Navigates back to the previous page.
            /// </summary>
            public async Task GoBackAsync()
            {
                await Shell.Current.GoToAsync("..");
            }

            /// <summary>
            /// Navigates to the root page.
            /// </summary>
            public async Task NavigateToRootAsync()
            {
                await Shell.Current.Navigation.PopToRootAsync();
            }

            /// <summary>
            /// Clears the navigation stack and navigates to a new route.
            /// </summary>
            public async Task ResetNavigationAsync(string route)
            {
                await Shell.Current.GoToAsync($"//{route}");
            }
        }
    }

}
