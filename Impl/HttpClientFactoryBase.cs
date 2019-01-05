using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;

namespace HttpClientFactory.Impl
{
    public abstract class HttpClientFactoryBase : IHttpClientFactory
    {
        private readonly ConcurrentDictionary<string, IHttpClient> _clients = new ConcurrentDictionary<string, IHttpClient>();
        public virtual IHttpClient GetHttpClient(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException(nameof(url));

            return _clients.AddOrUpdate(
                GetCacheKey(url),
                Create,
                (u, client) => client.IsDisposed ? Create(u) : client);
        }

        public void Dispose()
        {
            foreach (var kv in _clients)
            {
                if (!kv.Value.IsDisposed)
                    kv.Value.Dispose();
            }
            _clients.Clear();
        }


        protected abstract string GetCacheKey(string url);

        protected virtual IHttpClient Create(string url) => new SafeHttpClient(this,url);


        public virtual HttpClient CreateHttpClient(HttpMessageHandler handler)
        {
            return new HttpClient(handler);
        }

        public virtual HttpMessageHandler CreateMessageHandler()
        {
            return new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
        }
    }
}
