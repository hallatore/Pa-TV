using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pa_TV.Util
{
    public static class Format
    {
        public static Uri CreateLogoUriFromKey(string key)
        {
            return new Uri("http://m.get.no/rest/open/image/cms/resize?width=130&height=90&key=" + key);
        }
    }
}
