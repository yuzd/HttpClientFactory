using System;
using System.Net.Http;

namespace HttpClientFactory
{
    public interface IHttpClientFactory : IDisposable
    {
        IHttpClient GetHttpClient(string url);

        HttpClient CreateHttpClient(HttpMessageHandler handler);

        HttpMessageHandler CreateMessageHandler();
    }
}
