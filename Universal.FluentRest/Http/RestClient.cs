using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
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
            ContentHandlers = new Dictionary<string, IDeserializer>
            {
                {"*", new JsonDeserializer()},
                {"text/xml", new DotNetXmlDeserializer()},
                {"application/xml", new DotNetXmlDeserializer()},
                {"application/json", new JsonDeserializer()},
                {"text/json", new JsonDeserializer()},
                {"text/x-json", new JsonDeserializer()},
                {"text/javascript", new JsonDeserializer()}
            };
        }

        public bool DeserializeOnError { get; set; } = true;
        public string Url { get; set; }
        public HttpMethod Method { get; set; } = HttpMethod.Get;
        public Dictionary<string, string> UrlSegments { get; }
        public Dictionary<string, string> Parameters { get; }

        /// <summary>
        ///     Gets or sets the content of the body. (Will be serialized as JSON)
        /// </summary>
        /// <value>
        ///     The content of the body.
        /// </value>
        public object RequestContent { get; set; }

        public Dictionary<string, object> QueryParameters { get; }
        public Dictionary<string, string> Headers { get; }
        public Dictionary<string, IDeserializer> ContentHandlers { get; }

        public async Task<RestResponse> FetchResponseAsync()
        {
            using (var response = await InternalFetchResponseAsync().ConfigureAwait(false))
                return new RestResponse(response);
        }

        public async Task<RestResponse<T>> FetchResponseAsync<T>()
        {
            using (var response = await InternalFetchResponseAsync().ConfigureAwait(false))
                return await CreateResponseAsync<T>(response).ConfigureAwait(false);
        }

        protected async Task<HttpResponseMessage> InternalFetchResponseAsync()
        {
            using (var client = new HttpClient())
            {
                var supportsBodyContent = Method != HttpMethod.Get && Method != HttpMethod.Head &&
                                          Method != HttpMethod.Delete;
                using (var message = new HttpRequestMessage(Method, CreateUri(supportsBodyContent)))
                {
                    if (supportsBodyContent)
                    {
                        if (RequestContent != null)
                        {
                            var body = new JsonSerializer().Serialize(RequestContent);
                            message.Content = new StringContent(body, Encoding.UTF8, "application/json");
                        }
                        else if (Parameters.Count > 0)
                        {
                            var urlEncodedContent = new FormUrlEncodedContent(Parameters);
                            message.Content = urlEncodedContent;
                        }
                    }

                    foreach (var header in Headers)
                        message.Headers.TryAddWithoutValidation(header.Key, header.Value);

                    return await client.SendAsync(message).ConfigureAwait(false);
                }
            }
        }

        protected virtual async Task<RestResponse<T>> CreateResponseAsync<T>(HttpResponseMessage response)
        {
            if (!DeserializeOnError && !response.IsSuccessStatusCode)
                return new RestResponse<T>(response, default(T));

            var text = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var derserilizer = GetHandler(response.Content.Headers.ContentType?.MediaType);
            var derialized = derserilizer.Deserialize<T>(text);
            return new RestResponse<T>(response, derialized);
        }

        protected virtual IDeserializer GetHandler(string contentType)
        {
            IDeserializer deserializer;
            ContentHandlers.TryGetValue(contentType, out deserializer);
            return deserializer ?? ContentHandlers["*"];
        }

        protected virtual Uri CreateUri(bool supportsBody)
        {
            foreach (var urlSegment in UrlSegments)
            {
                Url = Url.Replace($"{{{urlSegment.Key}}}", WebUtility.UrlEncode(urlSegment.Value));
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