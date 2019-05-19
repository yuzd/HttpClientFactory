using System;
using System.Net.Http;

namespace HttpClientFactory.Impl
{
    internal class SafeHttpClient : IHttpClient
    {
        private readonly TimeSpan? _connectionLeaseTimeout;

        private readonly HttpClientFactoryBase _baseFactory;

        private Lazy<HttpClient> _httpClient;
        private Lazy<HttpMessageHandler> _httpMessageHandler;


        private DateTime? _clientCreatedAt;
        private HttpClient _zombieClient;

        private readonly object _connectionLeaseLock = new object();

        public HttpClient HttpClient => GetHttpClient();

        public HttpMessageHandler HttpMessageHandler => HttpRunTimeSeetings.Current?.HttpMessageHandler ?? _httpMessageHandler?.Value;
        public string BaseUrl { get; set; }
        public bool IsProxy { get; set; }

        public SafeHttpClient(HttpClientFactoryBase baseFactory,string baseUrl = null,bool isProxy = false)
        {
            _baseFactory = baseFactory;
            BaseUrl = baseUrl;
            IsProxy = isProxy;
            _connectionLeaseTimeout = HttpRunTimeSeetings.Current?.ConnectionLeaseTimeout ?? TimeSpan.FromMinutes(1);
            _httpClient = new Lazy<HttpClient>(CreateHttpClient);
            _httpMessageHandler = new Lazy<HttpMessageHandler>(()=>baseFactory.CreateMessageHandlerInternal(isProxy?baseUrl:null));
        }

        public bool IsDisposed { get; private set; }

        public virtual void Dispose()
        {
            if (IsDisposed)
                return;

            if (_httpMessageHandler?.IsValueCreated == true)
                _httpMessageHandler.Value.Dispose();
            if (_httpClient?.IsValueCreated == true)
                _httpClient.Value.Dispose();

            IsDisposed = true;
        }

        private HttpClient GetHttpClient()
        {
            if (ConnectionLeaseExpired())
            {
                lock (_connectionLeaseLock)
                {
                    if (ConnectionLeaseExpired())
                    {
                        _zombieClient?.Dispose();
                        _zombieClient = _httpClient.Value;
                        _httpClient = new Lazy<HttpClient>(CreateHttpClient);
                        _httpMessageHandler = new Lazy<HttpMessageHandler>(()=>_baseFactory.CreateMessageHandlerInternal(IsProxy ? BaseUrl : null));
                        _clientCreatedAt = DateTime.UtcNow;
                    }
                }
            }
            return _httpClient.Value;
        }

        private HttpClient CreateHttpClient()
        {
            var cli = _baseFactory.CreateHttpClientInternal(HttpMessageHandler);
            _clientCreatedAt = DateTime.UtcNow;
            return cli;
        }

        private bool ConnectionLeaseExpired()
        {
            // for thread safety, capture these to temp variables
            var createdAt = _clientCreatedAt;
            var timeout = _connectionLeaseTimeout;

            return
                _httpClient.IsValueCreated &&
                createdAt.HasValue &&
                timeout.HasValue &&
                DateTime.UtcNow - createdAt.Value > timeout.Value;
        }


    }
}
