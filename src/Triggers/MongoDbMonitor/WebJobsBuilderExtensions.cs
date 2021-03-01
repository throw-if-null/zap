using MediatR;
using MediatR.Pipeline;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDbMonitor;
using MongoDbMonitor.Commands.Common;
using MongoDbMonitor.Commands.Common.ExceptionHandlers.ExtractDocumentIdentifier;
using MongoDbMonitor.Commands.Common.ExceptionHandlers.ResolveCollectionType;
using MongoDbMonitor.Commands.Common.ExceptionHandlers.SendNotification;
using MongoDbMonitor.Commands.Exceptions;
using MongoDbMonitor.Commands.ExtractDocumentIdentifier;
using MongoDbMonitor.Commands.ProcessChangeEvent;
using MongoDbMonitor.Commands.ResolveCollectionType;
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
                typeof(ResolveCollectionTypeRequest),
                typeof(SendNotificationRequest)
            });

            builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ErrorHandlingPipelineBehavior<,>));

            builder.Services.AddScoped<
                IRequestExceptionHandler<ResolveCollectionTypeRequest, Unit, InvalidRequestTypeException>,
                InvalidRequestTypeExceptionHandler>();

            builder.Services.AddScoped<
                IRequestExceptionHandler<ResolveCollectionTypeRequest, Unit, MissingRequiredPropertyException>,
                MissingRequiredPropertyExceptionHandler>();

            builder.Services.AddScoped<
                IRequestExceptionHandler<ExtractDocumentIdentifierRequest, Unit, PropertyNotFoundInDocumentException>,
                PropertyNotFoundInDocumentExceptionHandler>();

            builder.Services.AddScoped<
                IRequestExceptionHandler<ExtractDocumentIdentifierRequest, Unit, InvalidObjectIdException>,
                InvalidObjectIdExceptionHandler>();

            builder.Services.AddScoped<
                IRequestExceptionHandler<SendNotificationRequest, Unit, SendNotificationFailedException>,
                SendNotificationFailedExceptionHandler>();

            builder.Services.AddScoped(typeof(IRequestExceptionHandler<,,>), typeof(GlobalExceptionHandler<,,>));

            builder.AddMongoDbTrigger();

            builder.Services.AddTransient<DbMonitor>();

            return builder;
        }

        public static IServiceCollection RegisterDocumentHandler<TRequest>(this IServiceCollection services)
            where TRequest : ExtractDocumentIdentifierRequest
        {
            services.AddMediatR(typeof(TRequest));

            return services;
        }
    }
}
