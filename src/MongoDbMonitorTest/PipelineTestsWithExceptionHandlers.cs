using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDbMonitor;
using MongoDbMonitor.Commands.Common.Responses;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MongoDbMonitorTest
{
    public class PipelineTestsWithExceptionHandlers
    {
        private static readonly Lazy<IServiceCollection> Services = new Lazy<IServiceCollection>(() => TestServiceFactory.RegisterServices(true, false), true);

        [Theory]
        [InlineData("items", "_id")]
        [InlineData("things", "SomeId")]
        public async Task Should_Return_SlackAlertSend_ProcessingStep(string collectionName, string requiredProperty)
        {
            await using var provider = Services.Value.BuildServiceProvider(true);

            var runner = provider.GetRequiredService<MonitorRunner>();

            var response = await runner.Run(
                collectionName,
                "update",
                new Dictionary<string, object>
                {
                    [requiredProperty] = ObjectId.GenerateNewId(),
                    ["name"] = "My brand",
                    ["accountId"] = ObjectId.GenerateNewId()
                },
                CancellationToken.None);

            Assert.False(response.IsSuccessfull);
            Assert.Equal(ProcessingStep.SendSlackAlert, response.FinalStep);
        }

        [Fact]
        public async Task Should_Return_ResolveCollectionType_ProcessingStep_On_InvalidRequestTypeException()
        {
            await using var provider = Services.Value.BuildServiceProvider(true);

            var runner = provider.GetRequiredService<MonitorRunner>();

            var response = await runner.Run(
                "Test",
                "update",
                new Dictionary<string, object>
                {
                    ["_id"] = ObjectId.GenerateNewId()
                },
                CancellationToken.None);

            Assert.False(response.IsSuccessfull);
            Assert.Equal(ProcessingStep.ResolveCollectionType, response.FinalStep);
        }

        [Fact]
        public async Task Should_Return_ResolveCollectionType_ProcessingStep_On_MissingRequiredPropertyException()
        {
            await using var provider = Services.Value.BuildServiceProvider(true);

            var runner = provider.GetRequiredService<MonitorRunner>();

            var response = await runner.Run(
                "Test2",
                "update",
                new Dictionary<string, object> { ["_id"] = ObjectId.GenerateNewId() },
                CancellationToken.None);

            Assert.False(response.IsSuccessfull);
            Assert.Equal(ProcessingStep.ResolveCollectionType, response.FinalStep);
        }

        [Fact]
        public async Task Should_Return_ExtractDocumentIdentifier_ProcessingStep_On_PropertyNotFoundInDocumentException()
        {
            await using var provider = Services.Value.BuildServiceProvider(true);

            var runner = provider.GetRequiredService<MonitorRunner>();

            var response = await runner.Run(
                "items",
                "update",
                new Dictionary<string, object> { ["nay_rolls"] = ObjectId.GenerateNewId() },
                CancellationToken.None);

            Assert.False(response.IsSuccessfull);
            Assert.Equal(ProcessingStep.ExtractDocumentIdentifier, response.FinalStep);
        }

        [Fact]
        public async Task Should_Return_ExtractDocumentIdentifier_ProcessingStep_On_InvalidObjectIdException()
        {
            await using var provider = Services.Value.BuildServiceProvider(true);

            var runner = provider.GetRequiredService<MonitorRunner>();

            var response = await runner.Run(
                "items",
                "update",
                new Dictionary<string, object> { ["_id"] = 1 },
                CancellationToken.None);

            Assert.False(response.IsSuccessfull);
            Assert.Equal(ProcessingStep.ExtractDocumentIdentifier, response.FinalStep);
        }

        [Theory]
        [InlineData("rename")]
        [InlineData("drop")]
        [InlineData("insert")]
        public async Task Should_Return_ProcessChangeEvent_ProcessingStep_For_Unconfigured_Operations(string operationName)
        {
            await using var provider = Services.Value.BuildServiceProvider(true);

            var runner = provider.GetRequiredService<MonitorRunner>();

            var response = await runner.Run(
                "items",
                operationName,
                new Dictionary<string, object> { ["_id"] = 1 },
                CancellationToken.None);

            Assert.False(response.IsSuccessfull);
            Assert.Equal(ProcessingStep.ProcessChangeEvent, response.FinalStep);
        }
    }
}