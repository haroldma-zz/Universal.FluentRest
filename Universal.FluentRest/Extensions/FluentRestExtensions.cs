using System.Net.Http;
using Universal.FluentRest.Deserializers;
using Universal.FluentRest.Http;

namespace Universal.FluentRest.Extensions
{
    public static class FluentRestExtensions
    {
        public static T Deserializer<T>(this T restObject, string contentType,
            IDeserializer deserializer) where T : RestClient
        {
            if (restObject.Deserializers.ContainsKey(contentType))
                restObject.Deserializers[contentType] = deserializer;
            else
                restObject.Deserializers.Add(contentType, deserializer);
            return restObject;
        }

        public static T Segment<T>(this T restObject, string name, string value) where T : RestClient
        {
            if (restObject.UrlSegments.ContainsKey(name))
                restObject.UrlSegments[name] = value;
            else
                restObject.UrlSegments.Add(name, value);
            return restObject;
        }

        public static T Query<T>(this T restObject, string name, object value) where T : RestClient
        {
            if (restObject.QueryParameters.ContainsKey(name))
                restObject.QueryParameters[name] = value;
            else
                restObject.QueryParameters.Add(name, value);
            return restObject;
        }

        public static T Parameter<T>(this T restObject, string name, string value) where T : RestClient
        {
            if (restObject.Parameters.ContainsKey(name))
                restObject.Parameters[name] = value;
            else
                restObject.Parameters.Add(name, value);
            return restObject;
        }

        public static T Url<T>(this T restObject, string url) where T : RestClient
        {
            restObject.Url = url;
            return restObject;
        }

        #region Http methods

        public static T Get<T>(this T restObject) where T : RestClient
        {
            restObject.Method = HttpMethod.Get;
            return restObject;
        }

        public static T Post<T>(this T restObject) where T : RestClient
        {
            restObject.Method = HttpMethod.Post;
            return restObject;
        }

        public static T Put<T>(this T restObject) where T : RestClient
        {
            restObject.Method = HttpMethod.Put;
            return restObject;
        }

        public static T Delete<T>(this T restObject) where T : RestClient
        {
            restObject.Method = HttpMethod.Delete;
            return restObject;
        }

        public static T Head<T>(this T restObject) where T : RestClient
        {
            restObject.Method = HttpMethod.Head;
            return restObject;
        }

        #endregion

    }
}