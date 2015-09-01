using System.Net;
using System.Net.Http;

namespace Universal.FluentRest.Http
{
    public sealed class RestResponse<T>
    {
        internal RestResponse(HttpResponseMessage response, T deserializedResponse)
        {
            StatusCode = response.StatusCode;
            ReasonPhrase = response.ReasonPhrase;
            IsSuccessStatusCode = response.IsSuccessStatusCode;
            DeserializedResponse = deserializedResponse;
        }

        public T DeserializedResponse { get; }
        public bool IsSuccessStatusCode { get; }
        public string ReasonPhrase { get; }
        public HttpStatusCode StatusCode { get; }
    }
}