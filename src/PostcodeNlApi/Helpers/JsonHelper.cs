using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace PostcodeNlApi.Helpers
{
    /// <summary>
    /// Summary description for JsonHelper
    /// </summary>
    public class JsonHelper
    {
        public static string Serialize<T>(T obj)
        {
            string retVal;
            var serializer = new DataContractJsonSerializer(obj.GetType(), new DataContractJsonSerializerSettings { DateTimeFormat = new DateTimeFormat("o") });
            using (var ms = new MemoryStream())
            {
                serializer.WriteObject(ms, obj);
                retVal = Encoding.Default.GetString(ms.ToArray());
            }
            return retVal;
        }

        public static T Deserialize<T>(string json)
        {
            var obj = Activator.CreateInstance<T>();
            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                var serializer = new DataContractJsonSerializer(obj.GetType());
                obj = (T)serializer.ReadObject(ms);
                ms.Close();
            }
            return obj;
        }
    }
}