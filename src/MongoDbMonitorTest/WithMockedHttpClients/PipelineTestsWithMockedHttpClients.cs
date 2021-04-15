using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDbFunction.Commands.ProcessItem;
using MongoDbFunction.Commands.ProcessThing;
using MongoDbMonitor;
using MongoDbMonitor.Commands.Common.Responses;
using MongoDbMonitor.Commands.ProcessChangeEvent;
using MongoDbMonitor.Commands.ResolveCollectionType;
using MongoDbMonitor.Commands.SendNotification;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MongoDbMonitorTest
{
    public class PipelineTestsWithMockedHttpClients
    {
        private static readonly Lazy<IServiceCollection> Services =
            new Lazy<IServiceCollection>(() => TestServiceFactory.RegisterServices(@"WithMockedHttpClients/test.json"), true);

        [Theory]
        [InlineData("items", "_id", nameof(ProcessItemRequest))]
        [InlineData("things", "SomeId", nameof(ProcessThingRequest))]
        public async Task Shoud_Return_NotifyStudio_ProcessingStep(string collectionName, string requiredProperty, string requestName)
        {
            await using var provider = Services.Value.BuildServiceProvider();

            var runner = provider.GetRequiredService<MonitorRunner>();

            var response = await runner.Run(collectionName,
                "update",
                new Dictionary<string, object>
                {
                    [requiredProperty] = ObjectId.GenerateNewId(),
                    ["name"] = "a name",
                    ["accountId"] = ObjectId.GenerateNewId()
                },
                CancellationToken.None);

            Assert.True(response.IsSuccessfull);
            Assert.Equal(ProcessingStep.Notify, response.FinalStep);
            Assert.Equal(4, response.Perf.Count);
            Assert.True(response.Perf.TryGetValue(nameof(ProcessChangeEventRequest), out _));
            Assert.True(response.Perf.TryGetValue(nameof(ResolveCollectionTypeRequest), out _));
            Assert.True(response.Perf.TryGetValue(requestName, out _));
            Assert.True(response.Perf.TryGetValue(nameof(SendNotificationRequest), out _));
        }
    }
}
