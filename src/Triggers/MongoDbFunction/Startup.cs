using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using MongoDbFunction.Commands.ProcessItem;
using MongoDbFunction.Commands.ProcessThing;

[assembly: FunctionsStartup(typeof(MongoDbFunction.Startup))]
namespace MongoDbFunction
{
    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.Services.RegisterDocumentHandler<ProcessItemRequest, ProcessItemHandler>();
            builder.Services.RegisterDocumentHandler<ProcessThingRequest, ProcessThingHandler>();

            builder.RegisterMonitor();
        }
    }
}
