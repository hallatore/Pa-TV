using System;
using System.Net.Http;
using System.Threading.Tasks;
using Pa_TV.Models;

namespace Pa_TV.Service
{
    public class CoreDataRetriever : IRetrieveCoreData
    {
        private static readonly Uri Endpoint = new Uri("http://devices.get.no/rest/open/tvguide/batch.json");
        private static readonly HttpClient Client = new HttpClient();

        public async Task<CoreData> GetCoreDataAsync()
        {
            var coreDataJson = await Client.GetStreamAsync(Endpoint);
            return CoreDataMapper.MapCoreData(coreDataJson);
        }
    }
}