using System.Collections.Generic;

namespace Pa_TV.Models
{
    public class CoreData
    {
        public IEnumerable<Channel> Channels { get; set; }
        public IEnumerable<ChannelGroup> ChannelGroups { get; set; }
        public IEnumerable<Genre> Genres { get; set; }
    }
}