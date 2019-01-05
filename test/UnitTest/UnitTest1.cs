using System;
using System.Net;
using System.Threading.Tasks;
using HttpClientFactory.Impl;
using Xunit;

namespace UnitTest
{
    public class UnitTest1
    {
        [Fact]
        public async Task Test_PerHostHttpClientFactory()
        {
            
            var factory = new PerHostHttpClientFactory();
            var client = factory.GetHttpClient("http://www.baidu.com");
            var str = await client.GetStringAsync("http://www.baidu.com");
        }
    }
}
