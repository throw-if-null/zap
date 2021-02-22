using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using MongoDbTrigger;

[assembly: FunctionsStartup(typeof(MongoDbFunction.Startup))]
namespace MongoDbFunction
{
    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.AddMongoDbTrigger();
        }
    }
}
