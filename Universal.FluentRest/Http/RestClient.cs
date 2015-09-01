using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Universal.FluentRest.Deserializers;

namespace Universal.FluentRest.Http
{
    public class RestClient
    {
        protected RestClient()
        {
            UrlSegments = new Dictionary<string, string>();
            QueryParameters = new Dictionary<string, object>();
            Parameters = new Dictionary<string, string>();
            Headers = new Dictionary<string, string>();
            Deserializers = new Dictionary<string, IDeserializer>
            {
                {"application/json", new JsonDeserializer()},
                {"text/json", new JsonDeserializer()}
            };
        }

        public IDeserializer DefaultDeserializer { get; set; } = new JsonDeserializer();

        public bool ParseOnlyOnSuccess { get; set; }
        public string Url { get; set; }
        public HttpMethod Method { get; set; } = HttpMethod.Get;
        public Dictionary<string, string> UrlSegments { get; }
        public Dictionary<string, string> Parameters { get; }
        public Dictionary<string, object> QueryParameters { get; }
        public Dictionary<string, string> Headers { get; }
        public Dictionary<string, IDeserializer> Deserializers { get; }

        public async Task<RestResponse<T>> FetchResponseAsync<T>()
        {
            using (var client = new HttpClient())
            {
                var supportsBodyContent = Method != HttpMethod.Get && Method != HttpMethod.Head &&
                                          Method != HttpMethod.Delete;
                using (var message = new HttpRequestMessage(Method, CreateUri(supportsBodyContent)))
                {
                    if (supportsBodyContent && Parameters.Count > 0)
                    {
                        var urlEncodedContent = new FormUrlEncodedContent(Parameters);
                        message.Content = urlEncodedContent;
                    }

                    foreach (var header in Headers)
                        message.Headers.TryAddWithoutValidation(header.Key, header.Value);

                    using (var response = await client.SendAsync(message))
                        return await CreateResponseAsync<T>(response);
                }
            }
        }

        protected virtual async Task<RestResponse<T>> CreateResponseAsync<T>(HttpResponseMessage response)
        {
            if (ParseOnlyOnSuccess && !response.IsSuccessStatusCode)
                return new RestResponse<T>(response, default(T));

            var text = await response.Content.ReadAsStringAsync();
            var derserilizer = GetDeserializer(response.Content.Headers.ContentType?.MediaType);
            var derialized = derserilizer.Deserialize<T>(text);
            return new RestResponse<T>(response, derialized);
        }

        protected virtual IDeserializer GetDeserializer(string contentType)
        {
            IDeserializer deserializer;
            Deserializers.TryGetValue(contentType, out deserializer);
            return deserializer ?? DefaultDeserializer;
        }

        protected virtual Uri CreateUri(bool supportsBody)
        {
            foreach (var urlSegment in UrlSegments)
            {
                Url = Url.Replace($"{{{urlSegment.Key}}}", urlSegment.Value);
            }

            foreach (var parameter in QueryParameters)
            {
                var seperator = "?";
                if (Url.Contains("?"))
                    seperator = "&";
                Url += $"{seperator}{parameter.Key}={parameter.Value}";
            }

            // If the request doesn't support body then add the parameters to the url.
            if (!supportsBody)
                foreach (var parameter in Parameters)
                {
                    var seperator = "?";
                    if (Url.Contains("?"))
                        seperator = "&";
                    Url += $"{seperator}{parameter.Key}={parameter.Value}";
                }

            return new Uri(Url);
        }
    }
}