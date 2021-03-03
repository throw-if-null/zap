using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbFunction.Commands.ProcessItem;
using MongoDbFunction.Commands.ProcessThing;
using MongoDbMonitor;
using MongoDbMonitor.Commands.Exceptions;
using MongoDbMonitor.Commands.ProcessChangeEvent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MongoDbMonitorTest
{
    public class DbMonitorTests
    {
        private static IServiceCollection RegisterService()
        {
            IConfiguration configuration =
                new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "test.json"), false)
                .Build();

            var services = new ServiceCollection();

            services.AddSingleton(configuration);

            services.AddLogging();

            services.RegisterOptions<Collection<CollectionOptions>>("MongoOptions:CollectionOptions");

            services.AddMemoryCache();

            services.RegisterMediator(ServiceLifetime.Transient);

            services.RegisterMediatorHandlers();
            services.RegisterExtractDocumentIdentifierHandler<ProcessItemRequest, ProcessItemHandler>();
            services.RegisterExtractDocumentIdentifierHandler<ProcessThingRequest, ProcessThingHandler>();

            return services;
        }

        [Fact]
        public async Task Should_Report()
        {
            using var provider = RegisterService().BuildServiceProvider();

            var data = new Dictionary<string, object>
            {
                ["_id"] = ObjectId.GenerateNewId(),
                ["name"] = "test",
                ["elapsed"] = 441
            };

            var handler = provider.GetRequiredService<IRequestHandler<ProcessChangeEventRequest, Unit>>();


            Unit response = await handler.Handle(
                new ProcessChangeEventRequest
                {
                    CollectionName = "items",
                     OperationType = ChangeStreamOperationType.Replace,
                     Values = data
                },
                CancellationToken.None);

            Assert.Equal(Unit.Value, response);
        }

        [Fact]
        public async Task Shoud_Throw_InvalidRequestTypeException()
        {
            using var provider = RegisterService().BuildServiceProvider();

            var data = new Dictionary<string, object>
            {
                ["_id"] = ObjectId.GenerateNewId(),
                ["name"] = "test",
                ["elapsed"] = 441
            };

            var handler = provider.GetRequiredService<IRequestHandler<ProcessChangeEventRequest, Unit>>();


            var exception = await Assert.ThrowsAsync<InvalidRequestTypeException>(() => handler.Handle(
                new ProcessChangeEventRequest
                {
                    CollectionName = "stuff",
                    OperationType = ChangeStreamOperationType.Replace,
                    Values = data
                },
                CancellationToken.None));
        }
    }
}
