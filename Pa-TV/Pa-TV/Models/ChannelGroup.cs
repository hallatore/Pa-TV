using System.Collections.Generic;

namespace Pa_TV.Models
{
    public class ChannelGroup
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<Channel> Channels { get; set; }
        public IEnumerable<string> ChannelIds { get; set; }
    }
}