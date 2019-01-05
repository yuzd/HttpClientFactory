using System;
using System.Net.Http;

namespace HttpClientFactory
{
    public interface IHttpClientFactory : IDisposable
    {
        HttpClient GetHttpClient(string url);

    }

}
