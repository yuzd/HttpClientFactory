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

        public virtual HttpClient GetProxiedHttpClient(string proxyUrl)
        {
            if (string.IsNullOrEmpty(proxyUrl))
                throw new ArgumentNullException(nameof(proxyUrl));

            var safeClient = _clients.AddOrUpdate(
                proxyUrl,
                CreateProxied,
                (u, client) => client.IsDisposed ? CreateProxied(u) : client);

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
        protected virtual IHttpClient CreateProxied(string proxyUrl) => new SafeHttpClient(this, proxyUrl, true);


        internal  HttpClient CreateHttpClientInternal(HttpMessageHandler handler)
        {
            return CreateHttpClient(handler);
        }

        internal HttpMessageHandler CreateMessageHandlerInternal(string proxyUrl = null)
        {
            return CreateMessageHandler(proxyUrl);
        }

        protected virtual HttpClient CreateHttpClient(HttpMessageHandler handler)
        {
            return new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(20)
            };
        }

        protected virtual HttpMessageHandler CreateMessageHandler(string proxyUrl = null)
        {
            if (!string.IsNullOrEmpty(proxyUrl))
            {
                return new HttpClientHandler
                {
                    UseProxy = true,
                    Proxy = new WebProxy(proxyUrl),
                    AutomaticDecompression = DecompressionMethods.None
                };
            }
            return new HttpClientHandler
            {
                UseProxy = false,
                AutomaticDecompression = DecompressionMethods.None
            };
        }
    }
}
