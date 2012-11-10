using System;
using System.Collections.Generic;

namespace Pa_TV.ViewModels
{
    public class MainPageViewModel
    {
        public DateTime Start { get; set; }
        public IEnumerable<string> Channels { get; set; }
    }
}
