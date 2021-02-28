using MediatR;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDbMonitor;
using MongoDbMonitor.Commands.Common;
using MongoDbMonitor.Commands.ProcessChangeEvent;
using MongoDbMonitor.Commands.ProcessDocument;
using MongoDbMonitor.Commands.ResolveCollection;
using MongoDbMonitor.Commands.SendNotification;
using MongoDbTrigger;
using System.Collections.ObjectModel;

namespace MongoDbFunction
{
    public static class WebJobsBuilderExtensions
    {
        public static IWebJobsBuilder RegisterMonitor(this IWebJobsBuilder builder)
        {
            builder
                .Services
                .AddOptions<Collection<CollectionOptions>>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.Bind("AzureFunctionsJobHost:MongoOptions:CollectionOptions", settings);
                });

            builder.Services.AddMemoryCache();

            builder.Services.AddLogging(x => x.AddConsole());

            builder.Services.AddMediatR(new[]
            {
                typeof(ProcessChangeEventRequest),
                typeof(ResolveCollectionRequest),
                typeof(SendNotificationRequest)
            });

            builder.Services.AddTransient<IRequestHandler<ProcessChangeEventRequest, Unit>, ProcessChangeEventHandler>();
            builder.Services.AddTransient<IRequestHandler<ResolveCollectionRequest, Unit>, ResolveCollectionHandler>();
            builder.Services.AddTransient<IRequestHandler<SendNotificationRequest, Unit>, SendNotificationHandler>();

            builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ErrorHandlingPipelineBehavior<,>));

            builder.AddMongoDbTrigger();

            builder.Services.AddTransient<DbMonitor>();

            return builder;
        }

        public static IServiceCollection RegisterDocumentHandler<TRequest, THandler>(this IServiceCollection services)
            where TRequest : ProcessDocumentRequest
            where THandler : class, IErrorHandlingRequestHanlder<TRequest, Unit>
        {
            services.AddMediatR(typeof(TRequest));

            services.AddTransient<IRequestHandler<TRequest, Unit>, THandler>();

            return services;
        }
    }
}
