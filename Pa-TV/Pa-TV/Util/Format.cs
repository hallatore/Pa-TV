using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pa_TV.Util
{
    public static class Format
    {
        public static Uri CreateLogoUriFromKey(string key)
        {
            return CreateLogoUriFromKey(key, 90,130);
        }
        public static Uri CreateLogoUriFromKey(string key, int height, int width)
        {
            return new Uri("http://m.get.no/rest/open/image/cms/resize?height="+height+"&width="+width+"&key=" + key);
        }

        public static DateTime ParseDate(string date)
        {
            return DateTime.ParseExact(date, App.DateFormat, CultureInfo.InvariantCulture);
        }
    }
}
