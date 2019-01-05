using System;
using System.Net.Http;

namespace HttpClientFactory
{
    public interface IHttpClient : IDisposable
    {
        HttpClient HttpClient { get; }

        HttpMessageHandler HttpMessageHandler { get; }

        string BaseUrl { get; set; }
        bool IsDisposed { get; }
    }
}
