using System;
using Windows.UI.Xaml.Data;

namespace Pa_TV.Converters
{
    public class Date : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTime)
                return ((DateTime) value).ToString("HH:mm");

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
