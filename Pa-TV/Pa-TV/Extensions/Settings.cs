using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Pa_TV.Extensions
{
    public static class Settings
    {
        public static string ToJson<T>(this T item)
        {
            var ms = new MemoryStream();
            var sw = new StreamReader(ms);
            var serializer = new DataContractJsonSerializer(typeof(T));
            serializer.WriteObject(ms, item);
            ms.Seek(0, SeekOrigin.Begin);
            return sw.ReadToEnd();
        }

        public static T ToObject<T>(this string json)
        {
            try
            {
                var ms = new MemoryStream();
                var sw = new StreamWriter(ms);
                sw.Write(json);
                sw.Flush();
                ms.Seek(0, SeekOrigin.Begin);
                var serializer = new DataContractJsonSerializer(typeof (T));
                return (T) serializer.ReadObject(ms);
            }
            catch(SerializationException ex)
            {
                return default(T);
            }
        }
    }
}
