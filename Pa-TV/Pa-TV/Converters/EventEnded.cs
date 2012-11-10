using System;
using Windows.UI.Xaml.Data;

namespace Pa_TV.Converters
{
    public class EventEnded : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool)
                return ((bool) value) ? 0.2 : 1;

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
