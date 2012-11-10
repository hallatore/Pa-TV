using System;
using System.Collections.Generic;
using Pa_TV.Models;

namespace Pa_TV.ViewModels
{
    public class MainPageViewModel
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public IEnumerable<string> Channels { get; set; }
        public IEnumerable<Channel> ChannelList { get; set; }
    }
}
