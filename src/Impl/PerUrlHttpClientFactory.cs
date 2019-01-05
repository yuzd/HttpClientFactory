namespace HttpClientFactory.Impl
{
    /// <summary>
    /// same url use same HttpClient
    /// </summary>
    public class PerUrlHttpClientFactory : HttpClientFactoryBase
    {
        protected override string GetCacheKey(string url)
        {
            return url;
        }

        protected override IHttpClient Create(string url)
        {
            return new SafeHttpClient(this, url);
        }
    }
}
