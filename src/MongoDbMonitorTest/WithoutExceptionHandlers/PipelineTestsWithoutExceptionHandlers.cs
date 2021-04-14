using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDbMonitor;
using MongoDbMonitor.Commands.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MongoDbMonitorTest
{
    public class PipelineTestsWithoutExceptionHandlers
    {
        private static readonly Lazy<IServiceCollection> Services =
            new Lazy<IServiceCollection>(() => TestServiceFactory.RegisterServices(false, @"WithoutExceptionHandlers/test.json"), true);

        [Theory]
        [InlineData("items", "_id")]
        [InlineData("things", "SomeId")]
        public async Task Should_Throw_SendNotificationFailedException(string collectionName, string requiredProperty)
        {
            await using var provider = Services.Value.BuildServiceProvider();

            var runner = provider.GetRequiredService<MonitorRunner>();

            await
                Assert.ThrowsAsync<SendNotificationFailedException>(
                    () =>
                        runner.Run(
                            collectionName,
                            "update",
                            new Dictionary<string, object>
                            {
                                [requiredProperty] = ObjectId.GenerateNewId(),
                                ["name"] = "My brand",
                                ["accountId"] = ObjectId.GenerateNewId()
                            },
                            CancellationToken.None));
        }

        [Fact]
        public async Task Shoud_Throw_InvalidRequestTypeException()
        {
            await using var provider = Services.Value.BuildServiceProvider();

            var runner = provider.GetRequiredService<MonitorRunner>();

            await
                Assert.ThrowsAsync<InvalidRequestTypeException>(
                    () =>
                        runner.Run(
                            "Test",
                            "update",
                            new Dictionary<string, object>
                            {
                                ["_id"] = ObjectId.GenerateNewId()
                            },
                            CancellationToken.None));
        }

        [Fact]
        public async Task Should_Throw_MissingRequiredPropertyException()
        {
            await using var provider = Services.Value.BuildServiceProvider();

            var runner = provider.GetRequiredService<MonitorRunner>();

            await
                Assert.ThrowsAsync<MissingRequiredPropertyException>(
                    () =>
                        runner.Run(
                            "Test2",
                            "update",
                            new Dictionary<string, object>
                            {
                                ["_id"] = ObjectId.GenerateNewId()
                            },
                            CancellationToken.None));
        }

        [Fact]
        public async Task Should_Throw_PropertyNotFoundInDocumentException()
        {
            await using var provider = Services.Value.BuildServiceProvider();

            var runner = provider.GetRequiredService<MonitorRunner>();

            await
                Assert.ThrowsAsync<PropertyNotFoundInDocumentException>(
                    () =>
                        runner.Run(
                            "items",
                            "update",
                            new Dictionary<string, object>
                            {
                                ["nay_rolls"] = ObjectId.GenerateNewId()
                            },
                            CancellationToken.None));
        }

        [Fact]
        public async Task Should_Throw_InvalidObjectIdException()
        {
            await using var provider = Services.Value.BuildServiceProvider();

            var runner = provider.GetRequiredService<MonitorRunner>();

            await
                Assert.ThrowsAsync<InvalidObjectIdException>(
                    () =>
                        runner.Run(
                            "items",
                            "update",
                            new Dictionary<string, object>
                            {
                                ["_id"] = 1
                            },
                            CancellationToken.None));
        }
    }
}
