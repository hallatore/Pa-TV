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
        Task<IEnumerable<Channel>> GetEventsTodayAsync(IEnumerable<string> channels);
        Task<IEnumerable<Channel>> GetEventsForDateAsync(DateTime dateTime, IEnumerable<string> channels = null);
    }
}