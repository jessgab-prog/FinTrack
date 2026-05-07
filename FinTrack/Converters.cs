using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace FinTrack
{
    public class TypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() switch
            {
                "Income" => new SolidColorBrush(Color.FromRgb(29, 158, 117)),
                "Expense" => new SolidColorBrush(Color.FromRgb(226, 75, 74)),
                _ => new SolidColorBrush(Color.FromRgb(26, 26, 26))
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class TypeToNotifColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() switch
            {
                "Danger" => new SolidColorBrush(Color.FromRgb(226, 75, 74)),
                "Warning" => new SolidColorBrush(Color.FromRgb(239, 159, 39)),
                "Success" => new SolidColorBrush(Color.FromRgb(29, 158, 117)),
                _ => new SolidColorBrush(Color.FromRgb(55, 138, 221))
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class BoolToVisConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool b && b
                ? System.Windows.Visibility.Collapsed
                : System.Windows.Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}