using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Pa_TV.Models;
using Pa_TV.Service;
using System.Linq;

namespace Pa_TV.ViewModels
{
    public class ChannelDetailsPageViewModel
    {
        private static readonly DateTime StartDate = DateTime.Today.AddHours(5);

        private readonly Channel _channel;
        private readonly IRetrieveEvents _eventsRetriever;

        public ObservableCollection<EventForDate> EventsForDate { get; set; }

        public ChannelDetailsPageViewModel(Channel channel)
        {
            _channel = channel;
            _eventsRetriever = new EventRetriever();

            EventsForDate = new ObservableCollection<EventForDate>();
        }

        public async void LoadData(int daysToLoad = 4)
        {
            for (var i = 0; i < daysToLoad; i++)
            {
                EventsForDate.Add(await GetEventsForChannelOnDate(StartDate.AddDays(i)));
            }
        }

        private async Task<EventForDate> GetEventsForChannelOnDate(DateTime dateTime)
        {
            var channels = await _eventsRetriever.GetEventsForDateAsync(dateTime, new[] {_channel.Id});
            var channel = channels.First();

            return new EventForDate
                   {
                       Date = dateTime,
                       Events = new ObservableCollection<Event>(channel.Events)
                   };
        }
    }
}