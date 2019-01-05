using System;
using System.Net.Http;

namespace HttpClientFactory.Impl
{


#if NET45
    [Serializable]
#endif
    public class HttpRunTimeSeetings : IDisposable
    {
        public HttpRunTimeSeetings()
        {
            SetCurrentTest(this);
        }

        public void Dispose()
        {
            SetCurrentTest(null);
        }

        public static HttpRunTimeSeetings Current => GetCurrentTest();

        public TimeSpan? Timeout { get; set; }
        public TimeSpan? ConnectionLeaseTimeout { get; set; }

        public HttpMessageHandler HttpMessageHandler { get; set; }


#if NET45
        private static void SetCurrentTest(HttpRunTimeSeetings runTimeSeetings) => System.Runtime.Remoting.Messaging.CallContext.LogicalSetData("HttpRunTimeSeetings", runTimeSeetings);
        private static HttpRunTimeSeetings GetCurrentTest() => System.Runtime.Remoting.Messaging.CallContext.LogicalGetData("HttpRunTimeSeetings") as HttpRunTimeSeetings;
#else
        private static readonly System.Threading.AsyncLocal<HttpRunTimeSeetings> _test = new System.Threading.AsyncLocal<HttpRunTimeSeetings>();
        private static void SetCurrentTest(HttpRunTimeSeetings test) => _test.Value = test;
        private static HttpRunTimeSeetings GetCurrentTest() => _test.Value;
#endif

    }
}
