using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using MongoDbFunction.Commands.ProcessItem;
using MongoDbFunction.Commands.ProcessThing;
using MongoDbMonitor;
using MongoDbTrigger;

[assembly: FunctionsStartup(typeof(MongoDbFunction.Startup))]
namespace MongoDbFunction
{
    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.Services.RegisterProcessDocumentMediatorHandler<ProcessItemRequest, ProcessItemHandler>();
            builder.Services.RegisterProcessDocumentMediatorHandler<ProcessThingRequest, ProcessThingHandler>();

            builder.Services.AddMongoDbCollectionMonitor();
            builder.AddMongoDbTrigger();
        }
    }
}
