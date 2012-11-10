using System;
using System.Collections.ObjectModel;
using Pa_TV.Common;

namespace Pa_TV.Models
{
    public class EventForDate : BindableBase
    {
        public EventForDate()
        {
            Events = new ObservableCollection<Event>();
        }

        private DateTime _date;
        public DateTime Date
        {
            get { return _date; }
            set { SetProperty(ref _date, value); }
        }

        public ObservableCollection<Event> Events { get; set; }
    }
}