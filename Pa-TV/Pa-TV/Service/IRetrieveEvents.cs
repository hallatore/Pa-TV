using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Pa_TV.Models;

namespace Pa_TV.Service
{
    public interface IRetrieveEvents
    {
        Task<IEnumerable<Channel>> GetEventsTodayAsync();
    }

    public class EventRetriever : IRetrieveEvents
    {
        private static readonly Uri EndpointUri = new Uri("http://devices.get.no/rest/open/tvguide/events.json");
        private static readonly HttpClient Client = new HttpClient();
        private const int MinutesInDay = 1440;

        public async Task<IEnumerable<Channel>> GetEventsTodayAsync()
        {
            var startOfTvDay = DateTime.Today.AddHours(5); // Today @ 5 'o clock in the morning

            var urlBuilder = new UriBuilder(EndpointUri)
                             {
                                 Query = string.Format("start={0}&duration={1}",
                                                       startOfTvDay.ToString(App.DateFormat), MinutesInDay)
                             };

            var jsonStream = await Client.GetStreamAsync(urlBuilder.Uri);

            return EventDataMapper.MapEvents(jsonStream);
        }
    }
}