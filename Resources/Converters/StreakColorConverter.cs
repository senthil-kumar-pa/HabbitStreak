namespace HabbitStreak.Resources.Converters
{
    using System;
    using System.Globalization;
    using Microsoft.Maui.Controls;

    public class StreakColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            int streak = (int)(value ?? 0);
            if (streak >= 10) return Color.FromArgb("#C8E6C9"); // Greenish
            if (streak >= 5) return Color.FromArgb("#FFF9C4");  // Yellowish
            return Color.FromArgb("#FFCDD2");                   // Reddish
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

}
