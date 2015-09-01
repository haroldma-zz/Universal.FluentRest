using Newtonsoft.Json;

namespace Universal.FluentRest.Deserializers
{
    public class JsonDeserializer : IDeserializer
    {
        public T Deserialize<T>(string text)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(text);
            }
            catch
            {
                return default(T);
            }
        }

        public string Serialize(object obj)
        {
            try
            {
                return JsonConvert.SerializeObject(obj);
            }
            catch
            {
                return null;
            }
        }
    }
}