using System;

namespace MongoDbMonitor.Clients.HttpApi
{
    public class HttpApiClientOptions
    {
        public Uri ClearCacheWebhook { get; set; }

        public int TimeoutInSeconds { get; set; }
    }
}
