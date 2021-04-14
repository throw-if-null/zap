using MediatR;
using MediatR.Pipeline;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using MongoDbMonitor.Clients.HttpApi;
using MongoDbMonitor.Clients.SlackApi;
using MongoDbMonitor.Commands.Common;
using MongoDbMonitor.Commands.Common.ExceptionHandlers.ExtractDocumentIdentifier;
using MongoDbMonitor.Commands.Common.ExceptionHandlers.ResolveCollectionType;
using MongoDbMonitor.Commands.Common.ExceptionHandlers.SendNotification;
using MongoDbMonitor.Commands.Common.Responses;
using MongoDbMonitor.Commands.Exceptions;
using MongoDbMonitor.Commands.ExtractDocumentIdentifier;
using MongoDbMonitor.Commands.ProcessChangeEvent;
using MongoDbMonitor.Commands.ResolveCollectionType;
using MongoDbMonitor.Commands.SendNotification;
using MongoDbMonitor.Commands.SendSlackAlert;
using MongoDbMonitor.CrossCutting.QoS;
using MongoDbTrigger;
using Scissors;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace MongoDbMonitor
{
    public static class WebJobExtensions
    {
        private const string ConfigurationBasePath = "AzureFunctionsJobHost:MonitorOptions";

        public static IWebJobsBuilder AddMongoDbCollectionMonitor(this IWebJobsBuilder builder)
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));

            RegisterOptions<Collection<CollectionOptions>>(builder.Services, $"{ConfigurationBasePath}:{nameof(CollectionOptions)}");
            RegisterOptions<RetryProviderOptions>(builder.Services, $"{ConfigurationBasePath}:{nameof(RetryProviderOptions)}");
            RegisterOptions<HttpApiClientOptions>(builder.Services, $"{ConfigurationBasePath}:{nameof(HttpApiClientOptions)}");
            RegisterOptions<SlackApiClientOptions>(builder.Services, $"{ConfigurationBasePath}:{nameof(SlackApiClientOptions)}");
            RegisterOptions<Collection<HttpRequestInterceptorOptions>>(builder.Services, $"{ConfigurationBasePath}:{nameof(HttpRequestInterceptorOptions)}");

            builder.Services.AddLogging(x => x.AddConsole());

            builder.Services.AddMemoryCache();

            builder.Services.AddTransient<HttpRequestInterceptor>();

            builder.Services.AddHttpClient<IHttpApiClient, HttpApiClient>().AddHttpMessageHandler<HttpRequestInterceptor>();
            builder.Services.AddHttpClient<ISlackApiClient, SlackApiClient>().AddHttpMessageHandler<HttpRequestInterceptor>();

            builder.Services.AddSingleton<IRetryProvider, RetryProvider>();

            builder.Services.AddTransient<MonitorRunner>();

            RegisterRequestHandler<ProcessChangeEventRequest, ProcessChangeEventHandler>(builder.Services);
            RegisterRequestHandler<ResolveCollectionTypeRequest, ResolveCollectionTypeHandler>(builder.Services);
            RegisterRequestHandler<SendNotificationRequest, SendNotificationHandler>(builder.Services);
            RegisterRequestHandler<SendSlackAlertRequest, SendSlackAlertHandler>(builder.Services);

            RegisterMediator(builder.Services, ServiceLifetime.Transient);

            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(MetricsCapturingPipelineBehavior<,>));
            RegisterMediatorBehaviors(builder.Services);

            RegisterMediatorExceptionHandlers(builder.Services);

            builder.AddMongoDbTrigger();

            return builder;
        }

        public static IWebJobsBuilder RegisterProcessDocumentMediatorHandler<TRequest, THandler>(this IWebJobsBuilder builder)
            where THandler : class, IRequestHandler<TRequest, ProcessingStatusResponse>
            where TRequest : ExtractDocumentIdentifierRequest, IRequest<ProcessingStatusResponse>
        {
            builder.Services.AddTransient<IRequestHandler<TRequest, ProcessingStatusResponse>, THandler>();

            RegisterExtractProcessDocumentExceptionHandlers<TRequest>(builder.Services);

            return builder;
        }

        internal static IServiceCollection RegisterExtractProcessDocumentExceptionHandlers<TRequest>(
            IServiceCollection services)
            where TRequest : ExtractDocumentIdentifierRequest, IRequest<ProcessingStatusResponse>
        {
            services.AddTransient<
                IRequestExceptionHandler<TRequest, ProcessingStatusResponse, PropertyNotFoundInDocumentException>,
                PropertyNotFoundInDocumentExceptionHandler<TRequest>>();

            services.AddTransient<
                IRequestExceptionHandler<TRequest, ProcessingStatusResponse, InvalidObjectIdException>,
                InvalidObjectIdExceptionHandler<TRequest>>();

            return services;
        }

        internal static IServiceCollection RegisterOptions<TOption>(IServiceCollection services, string path)
            where TOption : class
        {
            services
                .AddOptions<TOption>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.Bind(path, settings);
                });

            return services;
        }

        internal static IServiceCollection RegisterMediator(IServiceCollection services, ServiceLifetime lifetime)
        {
            services.TryAddTransient<ServiceFactory>(p => p.GetService);
            services.TryAdd(new ServiceDescriptor(typeof(IMediator), typeof(Mediator), lifetime));
            services.TryAdd(new ServiceDescriptor(typeof(ISender), sp => sp.GetService<IMediator>(), lifetime));
            services.TryAdd(new ServiceDescriptor(typeof(IPublisher), sp => sp.GetService<IMediator>(), lifetime));

            return services;
        }

        internal static IServiceCollection RegisterMediatorBehaviors(IServiceCollection services)
        {
            // Use TryAddTransientExact (see below), we dó want to register our Pre/Post processor behavior, even if (a more concrete)
            // registration for IPipelineBehavior<,> already exists. But only once.
            TryAddTransientExact(services, typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
            TryAddTransientExact(services, typeof(IPipelineBehavior<,>), typeof(RequestPostProcessorBehavior<,>));
            TryAddTransientExact(services, typeof(IPipelineBehavior<,>), typeof(RequestExceptionActionProcessorBehavior<,>));
            TryAddTransientExact(services, typeof(IPipelineBehavior<,>), typeof(RequestExceptionProcessorBehavior<,>));

            return services;

            static void TryAddTransientExact(IServiceCollection services, Type serviceType, Type implementationType)
            {
                if (services.Any(reg => reg.ServiceType == serviceType && reg.ImplementationType == implementationType))
                    return;

                services.AddTransient(serviceType, implementationType);
            }
        }

        internal static IServiceCollection RegisterRequestHandler<TRequest, THandler>(IServiceCollection services)
            where TRequest : IRequest<ProcessingStatusResponse>
            where THandler : class, IRequestHandler<TRequest, ProcessingStatusResponse>
        {
            services.AddTransient<IRequestHandler<TRequest, ProcessingStatusResponse>, THandler>();

            return services;
        }

        internal static IServiceCollection RegisterMediatorExceptionHandlers(IServiceCollection services)
        {
            services.AddTransient<
                IRequestExceptionHandler<ResolveCollectionTypeRequest, ProcessingStatusResponse, InvalidRequestTypeException>,
                InvalidRequestTypeExceptionHandler>();

            services.AddTransient<
                IRequestExceptionHandler<ResolveCollectionTypeRequest, ProcessingStatusResponse, MissingRequiredPropertyException>,
                MissingRequiredPropertyExceptionHandler>();

            services.AddTransient<
                IRequestExceptionHandler<SendNotificationRequest, ProcessingStatusResponse, SendNotificationFailedException>,
                SendNotificationFailedExceptionHandler>();

            services.AddTransient(typeof(IRequestExceptionHandler<,,>), typeof(GlobalExceptionHandler<,,>));

            return services;
        }
    }
}
