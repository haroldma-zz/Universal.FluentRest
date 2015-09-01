using System.Threading.Tasks;

namespace Universal.FluentRest.Http
{
    public class RestObject<T> : RestClient
    {
        public virtual Task<RestResponse<T>> ToResponseAsync()
        {
            return FetchResponseAsync<T>();
        }
    }
}