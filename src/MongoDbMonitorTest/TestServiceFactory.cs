using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDbFunction.Commands.ProcessItem;
using MongoDbFunction.Commands.ProcessThing;
using MongoDbMonitor;
using System.IO;

namespace MongoDbMonitorTest
{
    internal static class TestServiceFactory
    {
        private const string ConfigurationBasePath = "MonitorOptions";

        public static IServiceCollection RegisterServices(
            string jsonSettingsName = "test.json")
        {
            var services = new ServiceCollection();

            services.AddSingleton(BuildConfiguration(jsonSettingsName));

            services.RegisterProcessDocumentMediatorHandler<ProcessItemRequest, ProcessItemHandler>();
            services.RegisterProcessDocumentMediatorHandler<ProcessThingRequest, ProcessThingHandler>();

            services.AddMongoDbCollectionMonitor(ConfigurationBasePath);

            return services;
        }

        private static IConfiguration BuildConfiguration(string jsonSettingsName = "test.json")
        {
            var builder = new ConfigurationBuilder();

            builder.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), jsonSettingsName), false);

            return builder.Build();
        }
    }
}
