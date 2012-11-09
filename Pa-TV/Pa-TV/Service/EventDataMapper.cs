using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Pa_TV.Models;

namespace Pa_TV.Service
{
    public static class EventDataMapper
    {
        public static async Task<IEnumerable<Event>> MapEvents(Stream jsonStream)
        {
            var serializer = new DataContractJsonSerializer(typeof(eventRoot));
            var proxyObject =  (eventRoot)serializer.ReadObject(jsonStream);

            return new List<Event>();
        }


        #region Proxy types

        // ReSharper disable InconsistentNaming
        // ReSharper disable ClassNeverInstantiated.Global

        public class eventRoot
        {
            public programEventsQueryResponse programEventsQueryResponse { get; set; }
        }

        public class programEventsQueryResponse
        {
            public channelWithEvents[] channels { get; set; }
        }

        public class channelWithEvents
        {
            public string id { get; set; }
            public string name { get; set; }
            public eventProxy[] events { get; set; }
            public string logoBlackBgKey { get; set; }
        }

        public class eventProxy
        {
            public string id { get; set; }
            public string title { get; set; }
            public string description { get; set; }
            public int duration { get; set; }
            public string start { get; set; }
        }

        // ReSharper restore ClassNeverInstantiated.Global
        // ReSharper restore InconsistentNaming

        #endregion
    }
}