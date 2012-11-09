﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pa_TV.Models
{
    public class Event
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

    }
}