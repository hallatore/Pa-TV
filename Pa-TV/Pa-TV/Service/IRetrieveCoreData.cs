using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pa_TV.Models;

namespace Pa_TV.Service
{
    public interface IRetrieveCoreData
    {
        Task<CoreData> GetCoreDataAsync();
    }
}
