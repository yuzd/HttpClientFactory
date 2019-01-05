using System;

namespace HttpClientFactory.Impl
{
    /// <summary>
    /// same Host use same HttpClient
    /// </summary>
    public class PerHostHttpClientFactory : HttpClientFactoryBase
    {
        protected override string GetCacheKey(string url)
        {
            return new Uri(url).Host;
        }
    }
}
