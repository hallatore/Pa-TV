using System;
using System.Collections.Generic;
using System.ComponentModel;
using Pa_TV.Models;

namespace Pa_TV.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public DateTime End { get; set; }

        public IEnumerable<string> Channels { get; set; }
        public IEnumerable<string> Favorites { get; set; }
        public IEnumerable<Channel> ChannelList { get; set; }

        private DateTime start;
        public DateTime Start
        {
            get { return start; }
            set
            {
                if (value != start)
                {
                    start = value;
                    NotifyPropertyChanged("Start");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
