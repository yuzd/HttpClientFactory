using System;
using System.Net;
using System.Threading.Tasks;
using HttpClientFactory.Impl;
using Xunit;

namespace UnitTest
{
    public class PerUrlHttpClientFactoryTests
    {
        [Fact]
        public void Should_Set_Client_Timeout_To_Default()
        {
            
            var factory = new PerUrlHttpClientFactory();
            var client = factory.GetHttpClient("http://www.baidu.com");
            Assert.Equal(client.Timeout, TimeSpan.FromSeconds(20));
        }

        [Fact]
        public void Should_Set_Client_Timeout()
        {
            
            var factory = new PerUrlHttpClientFactory(TimeSpan.FromMinutes(15));
            var client = factory.GetHttpClient("http://www.baidu.com");
            Assert.Equal(client.Timeout, TimeSpan.FromMinutes(15));
        }
    }
}
