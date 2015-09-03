using System.Threading.Tasks;

namespace Universal.FluentRest.Http
{
    public class RestRequestObject<T> : RestClient
    {
        public virtual Task<RestResponse<T>> ToResponseAsync()
        {
            return FetchResponseAsync<T>();
        }
    }

    public class RestRequestObject : RestClient
    {
        public virtual Task<RestResponse> ToResponseAsync()
        {
            return FetchResponseAsync();
        }
    }
}