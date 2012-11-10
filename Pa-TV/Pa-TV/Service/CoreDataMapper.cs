using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Pa_TV.Models;
using Pa_TV.Util;

namespace Pa_TV.Service
{
    public static class CoreDataMapper
    {
        public static CoreData MapCoreData(Stream jsonStream)
        {
            var serializer = new DataContractJsonSerializer(typeof (coreDataRoot));
            var proxyObject = (coreDataRoot) serializer.ReadObject(jsonStream);

            var channels = proxyObject.tvguideBatchResponse.channelQueryResponse.channels;
            var genres = proxyObject.tvguideBatchResponse.eventGenreQueryResponse.eventGenres;

            return new CoreData {Channels = channels.Select(MapChannel), Genres = genres.Select(MapGenre)};
        }

        private static Channel MapChannel(channel c)
        {
            return new Channel
                   {
                       Id = c.id,
                       Name = c.name,
                       LogoUrl = Format.CreateLogoUriFromKey(c.logoBlackBgKey),
                       LogoUrlBig = Format.CreateLogoUriFromKey(c.logoBlackBgKey, 0, 140)
                   };
        }
        private static Genre MapGenre(genre g)
        {
            return new Genre
            {
                Id = g.id,
                Name = g.name,
            };
        }
    }

    #region Proxy types

    // ReSharper disable InconsistentNaming
    // ReSharper disable ClassNeverInstantiated.Global

    public class coreDataRoot
    {
        public tvguideBatchResponse tvguideBatchResponse { get; set; }
    }

    public class eventGenreQueryResponse
    {
        public genre[] eventGenres { get; set; }
    }
    public class tvguideBatchResponse
    {
        public channelQueryResponse channelQueryResponse { get; set; }
        public eventGenreQueryResponse eventGenreQueryResponse { get; set; }
    }

    public class channelQueryResponse
    {
        public channel[] channels { get; set; }
    }

    public class genre
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class channel
    {
        public string id { get; set; }
        public string name { get; set; }
        public string logoBlackBgKey { get; set; }
    }

    // ReSharper restore ClassNeverInstantiated.Global
    // ReSharper restore InconsistentNaming

    #endregion
}