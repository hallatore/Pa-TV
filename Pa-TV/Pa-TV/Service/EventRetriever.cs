using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Pa_TV.Models;

namespace Pa_TV.Service
{
    public class EventRetriever : IRetrieveEvents
    {
        private static readonly Uri EndpointUri = new Uri("http://devices.get.no/rest/open/tvguide/events.json");
        private static readonly HttpClient Client = new HttpClient();
        private const int MinutesInDay = 1440;

        public async Task<IEnumerable<Channel>> GetEventsTodayAsync(DateTime start, IEnumerable<string> channels = null)
        {
            var channelUri = string.Empty;

            if (channels != null && channels.Count() > 0)
                channelUri = channels.Aggregate("&channels=", (current, channel) => current + (channel + ","));

            var urlBuilder = new UriBuilder(EndpointUri)
            {
                Query = string.Format("start={0}&duration={1}{2}", start.ToString(App.DateFormat), MinutesInDay, channelUri)
            };

            var jsonStream = await Client.GetStreamAsync(urlBuilder.Uri);
            return EventDataMapper.MapEvents(jsonStream);
        }
    }
    
    public class CachingEventRetriever : IRetrieveEvents
    {
        private static IEnumerable<Channel> cache = null;
        protected EventRetriever NonCahcingImplementation { get; set; }

        public CachingEventRetriever()
        {
            this.NonCahcingImplementation = new EventRetriever();
        }

        public async Task<IEnumerable<Channel>> GetEventsTodayAsync(DateTime start, IEnumerable<string> channels = null)
        {
            if (cache != null)
                return cache;

            cache = await NonCahcingImplementation.GetEventsTodayAsync(start, channels);
            return cache;
        }
    }
}