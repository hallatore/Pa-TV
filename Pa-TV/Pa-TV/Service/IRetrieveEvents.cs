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
        Task<IEnumerable<Event>> GetEventsTodayAsync();
    }

    public class EventRetriever : IRetrieveEvents
    {
        private static readonly Uri EndpointUri = new Uri("http://devices.get.no/rest/open/tvguide/events.json");
        private static readonly HttpClient Client = new HttpClient();
        private const int MinutesInDay = 1440;

        public async Task<IEnumerable<Event>> GetEventsTodayAsync()
        {
            var startOfTvDay = DateTime.Today.AddHours(5); // Today @ 5 'o clock in the morning

            var urlBuilder = new UriBuilder(EndpointUri);
            urlBuilder.Query = string.Format("start={0}&duration={1}",
                                             startOfTvDay.ToString("yyyy-MM-ddThh:mm"), MinutesInDay);

            var jsonStream = await Client.GetStreamAsync(urlBuilder.Uri);

            return await EventDataMapper.MapEvents(jsonStream);
        }
    }
}