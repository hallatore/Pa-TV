using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Pa_TV.Models;

namespace Pa_TV.Service
{
    public interface IRetrieveCoreData
    {
        Task<CoreData> GetCoreDataAsync();
    }

    public class CoreDataRetriever : IRetrieveCoreData
    {
        public async Task<CoreData> GetCoreDataAsync()
        {
            var uri = new Uri("http://devices.get.no/rest/open/tvguide/batch.json");
            var client = new HttpClient();

            var coreDataJson = await client.GetStreamAsync(uri);

            var serializer = new DataContractJsonSerializer(typeof(root));
            var obj = (root) serializer.ReadObject(coreDataJson);

            return new CoreData();
        }
    }

}

