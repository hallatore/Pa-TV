using System;
using System.ComponentModel;

namespace Pa_TV.Models
{
    public class Event : INotifyPropertyChanged
    {
        public string Id { get; set; }
        public int GenreId { get; set; }
        //public Genre Genre { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public int Duration { get; set; }

        public DateTime End
        {
            get { return Start.AddMinutes(Duration); }
        }

        public bool Ended
        {
            get { return DateTime.Now > End; }
        }

        private bool highLight;
        public bool HighLight
        {
            get { return highLight; }
            set
            {
                if (value != highLight)
                {
                    highLight = value;
                    NotifyPropertyChanged("HighLight");
                }
            }
        }

        private bool favorite;
        public bool Favorite
        {
            get { return favorite; }
            set
            {
                if (value != favorite)
                {
                    favorite = value;
                    NotifyPropertyChanged("Favorite");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}