using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Pa_TV.Converters
{
    public class FavoriteTitleBrush : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var boolValue = value as bool?;
            if (boolValue != null)
                return (bool)boolValue ? Application.Current.Resources["AppBrushHighlight"] : new SolidColorBrush(Colors.White);

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
