using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using MongoDbFunction.Commands.ProcessItem;
using MongoDbFunction.Commands.ProcessThing;
using MongoDbMonitor;

[assembly: FunctionsStartup(typeof(MongoDbFunction.Startup))]
namespace MongoDbFunction
{
    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.RegisterProcessDocumentMediatorHandler<ProcessItemRequest, ProcessItemHandler>();
            builder.RegisterProcessDocumentMediatorHandler<ProcessThingRequest, ProcessThingHandler>();

            builder.AddMongoDbCollectionMonitor();
        }
    }
}
