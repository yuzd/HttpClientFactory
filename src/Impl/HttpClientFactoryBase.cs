using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;

namespace HttpClientFactory.Impl
{
    public abstract class HttpClientFactoryBase : IHttpClientFactory
    {
        private readonly ConcurrentDictionary<string, IHttpClient> _clients = new ConcurrentDictionary<string, IHttpClient>();
        public virtual HttpClient GetHttpClient(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException(nameof(url));

            var safeClient = _clients.AddOrUpdate(
                GetCacheKey(url),
                Create,
                (u, client) => client.IsDisposed ? Create(u) : client);

            return safeClient.HttpClient;
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


        internal  HttpClient CreateHttpClientInternal(HttpMessageHandler handler)
        {
            return CreateHttpClient(handler);
        }

        internal HttpMessageHandler CreateMessageHandlerInternal()
        {
            return CreateMessageHandler();
        }

        protected virtual HttpClient CreateHttpClient(HttpMessageHandler handler)
        {
            return new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(200)
            };
        }

        protected virtual HttpMessageHandler CreateMessageHandler()
        {
            return new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
        }
    }
}
