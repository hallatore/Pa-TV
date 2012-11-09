using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Pa_TV.Models;

namespace Pa_TV.Service
{
    public static class CoreDataMapper
    {
        public static async Task<CoreData> MapCoreData(Stream jsonStream)
        {
            var serializer = new DataContractJsonSerializer(typeof(root));
            var proxyObject = (root)serializer.ReadObject(jsonStream);
            CoreData cd = new CoreData();
            cd.Channels = proxyObject.tvguideBatchResponse.channelQueryResponse.channels.Select(c =>
            {
                return new Channel
                {
                    Id = c.id,
                    Name = c.name,
                    LogoUrl = new Uri("http://m.get.no/rest/open/image/cms/resize?width=130&height=90&key=" + c.logoBlackBgKey)
                };
            });
            return cd;
        }
    }

    #region Proxy types

    // ReSharper disable InconsistentNaming
    // ReSharper disable ClassNeverInstantiated.Global

    public class root
    {
        public tvguideBatchResponse tvguideBatchResponse { get; set; }
    }

    public class tvguideBatchResponse
    {
        public channelQueryResponse channelQueryResponse { get; set; }
    }

    public class channelQueryResponse
    {
        public channel[] channels { get; set; }
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
