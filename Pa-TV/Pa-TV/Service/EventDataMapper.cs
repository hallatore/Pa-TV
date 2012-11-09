using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Pa_TV.Models;
using Pa_TV.Util;

namespace Pa_TV.Service
{
    public static class EventDataMapper
    {
        public static IEnumerable<Channel> MapEvents(Stream jsonStream)
        {
            var serializer = new DataContractJsonSerializer(typeof (eventRoot));
            var proxyObject = (eventRoot) serializer.ReadObject(jsonStream);

            var channels = proxyObject.programEventsQueryResponse.channels;

            var channelsWithEvents = channels.Select(MapChannel).ToList();

            return channelsWithEvents;
        }

        private static Channel MapChannel(channelWithEvents c)
        {
            return new Channel
                       {
                           Id = c.id,
                           Name = c.name,
                           Events = (c.events != null) ? c.events.Select(MapEvent).ToList() : Enumerable.Empty<Event>(),
                           LogoUrl = Format.CreateLogoUriFromKey(c.logoBlackBgKey)
                       };
        }

        private static Event MapEvent(eventProxy e)
        {
            return new Event
                       {
                           Id = e.id,
                           Title = e.title,
                           Description = e.description,
                           Start = Format.ParseDate(e.start),
                           Duration = e.duration,
                       };
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