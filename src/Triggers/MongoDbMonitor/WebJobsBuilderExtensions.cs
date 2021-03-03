using MediatR;
using MediatR.Pipeline;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDbMonitor.Commands.Common;
using MongoDbMonitor.Commands.Common.ExceptionHandlers.ExtractDocumentIdentifier;
using MongoDbMonitor.Commands.Common.ExceptionHandlers.ResolveCollectionType;
using MongoDbMonitor.Commands.Common.ExceptionHandlers.SendNotification;
using MongoDbMonitor.Commands.Exceptions;
using MongoDbMonitor.Commands.ExtractDocumentIdentifier;
using MongoDbMonitor.Commands.ProcessChangeEvent;
using MongoDbMonitor.Commands.ResolveCollectionType;
using MongoDbMonitor.Commands.SendNotification;
using MongoDbMonitor.Commands.SendSlackAlert;
using MongoDbTrigger;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MongoDbMonitor
{
    public static class WebJobsBuilderExtensions
    {
        public static IWebJobsBuilder RegisterMonitor(this IWebJobsBuilder builder)
        {
            builder.Services.RegisterOptions<Collection<CollectionOptions>>(
                "AzureFunctionsJobHost:MongoOptions:CollectionOptions");

            builder.Services.AddMemoryCache();

            builder.Services.AddLogging(x => x.AddConsole());

            builder.Services.RegisterMediator(ServiceLifetime.Scoped);
            builder.Services.RegisterMediatorExceptionBehviors();
            builder.Services.RegisterMediatorHandlers();

            builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(MetricsCapturingPipelineBehavior<,>));

            builder.AddMongoDbTrigger();

            builder.Services.AddTransient<DbMonitor>();

            return builder;
        }

        internal static IServiceCollection RegisterMediator(this IServiceCollection services, ServiceLifetime lifetime)
        {
            services.TryAddTransient<ServiceFactory>(p => p.GetService);
            services.TryAdd(new ServiceDescriptor(typeof(IMediator), typeof(Mediator), lifetime));
            services.TryAdd(new ServiceDescriptor(typeof(ISender), sp => sp.GetService<IMediator>(), lifetime));
            services.TryAdd(new ServiceDescriptor(typeof(IPublisher), sp => sp.GetService<IMediator>(), lifetime));

            // Use TryAddTransientExact (see below), we dó want to register our Pre/Post processor behavior, even if (a more concrete)
            // registration for IPipelineBehavior<,> already exists. But only once.
            TryAddTransientExact(services, typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
            TryAddTransientExact(services, typeof(IPipelineBehavior<,>), typeof(RequestPostProcessorBehavior<,>));
            TryAddTransientExact(services, typeof(IPipelineBehavior<,>), typeof(RequestExceptionActionProcessorBehavior<,>));
            TryAddTransientExact(services, typeof(IPipelineBehavior<,>), typeof(RequestExceptionProcessorBehavior<,>));

            return services;

            static void TryAddTransientExact(IServiceCollection s, Type serviceType, Type implementationType)
            {
                if (s.Any(reg => reg.ServiceType == serviceType && reg.ImplementationType == implementationType))
                    return;

                s.AddTransient(serviceType, implementationType);
            }
        }

        public static IServiceCollection RegisterExtractDocumentIdentifierHandler<TRequest, THandler>(this IServiceCollection services)
            where TRequest : ExtractDocumentIdentifierRequest
            where THandler : class, IRequestHandler<TRequest, Unit>
        {
            services.AddTransient<IRequestHandler<TRequest, Unit>, THandler>();

            return services;
        }

        internal static void RegisterOptions<TOptions>(
            this IServiceCollection services,
            string path)
            where TOptions : class, new()
        {
            var key = path;

            services
                .AddOptions<TOptions>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.Bind(key, settings);
                });
        }

        internal static IServiceCollection RegisterMediatorExceptionBehviors(this IServiceCollection services)
        {
            services.AddScoped<
                IRequestExceptionHandler<ResolveCollectionTypeRequest, Unit, InvalidRequestTypeException>,
                InvalidRequestTypeExceptionHandler>();

            services.AddScoped<
                IRequestExceptionHandler<ResolveCollectionTypeRequest, Unit, MissingRequiredPropertyException>,
                MissingRequiredPropertyExceptionHandler>();

            services.AddScoped<
                IRequestExceptionHandler<ExtractDocumentIdentifierRequest, Unit, PropertyNotFoundInDocumentException>,
                PropertyNotFoundInDocumentExceptionHandler>();

            services.AddScoped<
                IRequestExceptionHandler<ExtractDocumentIdentifierRequest, Unit, InvalidObjectIdException>,
                InvalidObjectIdExceptionHandler>();

            services.AddScoped<
                IRequestExceptionHandler<SendNotificationRequest, Unit, SendNotificationFailedException>,
                SendNotificationFailedExceptionHandler>();

            services.AddScoped(typeof(IRequestExceptionHandler<,,>), typeof(GlobalExceptionHandler<,,>));

            return services;
        }

        internal static IServiceCollection RegisterMediatorHandlers(this IServiceCollection services)
        {
            services.AddTransient<IRequestHandler<ProcessChangeEventRequest, Unit>, ProcessChangeEventHandler>();
            services.AddTransient<IRequestHandler<ResolveCollectionTypeRequest, Unit>, ResolveCollectionTypeHandler>();
            services.AddTransient<IRequestHandler<SendNotificationRequest, Unit>, SendNotificationHandler>();
            services.AddTransient<IRequestHandler<SendSlackAlertRequest, Unit>, SendSlackAlertHandler>();

            return services;
        }
    }
}
