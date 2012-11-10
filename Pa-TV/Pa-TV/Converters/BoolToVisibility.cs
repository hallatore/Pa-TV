using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Pa_TV.Converters
{
    public class BoolToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var boolValue = value as bool?;
            if (boolValue != null)
                return (bool)boolValue ? Visibility.Visible : Visibility.Collapsed;

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
