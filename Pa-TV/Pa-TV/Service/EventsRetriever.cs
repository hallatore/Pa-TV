using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;

namespace Pa_TV.Service
{
    class EventsRetriever : IRetrieveEvents
    {
        public Task<IEnumerable<Models.Event>> GetAllEvents()
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer     
        }
    }
}
