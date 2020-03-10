using System;

namespace HttpClientFactory.Impl
{
    /// <summary>
    /// same Host use same HttpClient
    /// </summary>
    public class PerHostHttpClientFactory : HttpClientFactoryBase
    {
        public PerHostHttpClientFactory()
        {
        }

        public PerHostHttpClientFactory(TimeSpan defaultClientTimeout) : base(defaultClientTimeout)
        {
        }

        protected override string GetCacheKey(string url)
        {
            return new Uri(url).Host;
        }
    }
}
