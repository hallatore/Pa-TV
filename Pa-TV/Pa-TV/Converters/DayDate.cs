using System;
using Windows.UI.Xaml.Data;

namespace Pa_TV.Converters
{
    public class DayDate : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTime)
            {
                var date = (DateTime) value;

                if (date.Date == DateTime.Today)
                    return "i dag";

                if (date.Date == DateTime.Today.AddDays(1))
                    return "i morgen";

                return date.ToString("dddd - dd.MM");
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
