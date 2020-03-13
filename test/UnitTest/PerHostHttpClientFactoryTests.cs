using System;
using HttpClientFactory.Impl;
using Xunit;

namespace UnitTest
{
    public class PerHostHttpClientFactoryTests
    {
        [Fact]
        public void Should_Set_Client_Timeout_To_Default()
        {
            
            var factory = new PerHostHttpClientFactory();
            var client = factory.GetHttpClient("http://www.baidu.com");
            Assert.Equal(client.Timeout, TimeSpan.FromSeconds(100));
        }

        [Fact]
        public void Should_Set_Client_Timeout()
        {
            
            var factory = new PerHostHttpClientFactory(TimeSpan.FromMinutes(15));
            var client = factory.GetHttpClient("http://www.baidu.com");
            Assert.Equal(client.Timeout, TimeSpan.FromMinutes(15));
        }
    }
}
