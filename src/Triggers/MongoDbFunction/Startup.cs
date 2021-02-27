using MediatR;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDbFunction.Commands.ProcessDbEvent;
using MongoDbFunction.Commands.ProcessDocument;
using MongoDbFunction.Commands.ProcessItem;
using MongoDbFunction.Commands.ProcessThing;
using MongoDbFunction.Commands.SendNotification;
using MongoDbTrigger;
using System.Collections.ObjectModel;

[assembly: FunctionsStartup(typeof(MongoDbFunction.Startup))]
namespace MongoDbFunction
{
    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder
                .Services
                .AddOptions<Collection<CollectionOptions>>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.Bind("AzureFunctionsJobHost:MongoOptions:CollectionOptions", settings);
                });

            builder.Services.AddMediatR(new[]
            {
                typeof(ProcessDbEventRequest),
                typeof(ProcessDocumentRequest),
                typeof(ProcessItemRequest),
                typeof(ProcessThingRequest),
                typeof(SendNotificationRequest)
            });

            builder.Services.AddTransient<IRequestHandler<ProcessDbEventRequest, Unit>, ProcessDbEventHandler>();
            builder.Services.AddTransient<IRequestHandler<ProcessDocumentRequest>, ProcessDocumentHandler>();
            builder.Services.AddTransient<IRequestHandler<ProcessItemRequest, Unit>, ProcessItemHandler>();
            builder.Services.AddTransient<IRequestHandler<ProcessThingRequest, Unit>, ProcessThingHandler>();
            builder.Services.AddTransient<IRequestHandler<SendNotificationRequest, Unit>, SendNotificationHandler>();

            builder.AddMongoDbTrigger();
        }
    }
}
