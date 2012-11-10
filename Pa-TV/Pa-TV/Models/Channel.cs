using System;
using System.Collections.Generic;

namespace Pa_TV.Models
{
    public class Channel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Uri LogoUrl { get; set; }
        public Uri LogoUrlBig { get; set; }
        public IEnumerable<Event> Events { get; set; }
    }
}