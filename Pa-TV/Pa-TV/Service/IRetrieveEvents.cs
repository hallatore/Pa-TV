using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pa_TV.Models;

namespace Pa_TV.Service
{
    public interface IRetrieveEvents
    {
        Task<IEnumerable<Channel>> GetEventsForDateAsync(DateTime start, IEnumerable<string> channels);
    }
}