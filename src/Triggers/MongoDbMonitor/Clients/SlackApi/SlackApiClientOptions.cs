using System;

namespace MongoDbMonitor.Clients.SlackApi
{
    public class SlackApiClientOptions
    {
        public Uri ChannelWebhookUrl { get; set; }

        public int TimeoutInSeconds { get; set; }

    }
}
