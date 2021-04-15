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
using MongoDbMonitor.Commands.Common.ExceptionHandlers;
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
using Scissors;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace MongoDbMonitor
{
    public static class WebJobExtensions
    {
        private const string ConfigurationBasePath = "AzureFunctionsJobHost:MonitorOptions";

        public static IServiceCollection AddMongoDbCollectionMonitor(
            this IServiceCollection services,
            string configurationBasePath = ConfigurationBasePath)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            RegisterOptions(services, configurationBasePath);

            services.AddLogging(x => x.AddConsole());

            services.AddMemoryCache();

            services.AddTransient<HttpRequestInterceptor>();

            services.AddHttpClient<IHttpApiClient, HttpApiClient>().AddHttpMessageHandler<HttpRequestInterceptor>();
            services.AddHttpClient<ISlackApiClient, SlackApiClient>().AddHttpMessageHandler<HttpRequestInterceptor>();

            services.AddSingleton<IRetryProvider, RetryProvider>();

            services.AddTransient<MonitorRunner>();

            RegisterRequestHandler<ProcessChangeEventRequest, ProcessChangeEventHandler>(services);
            RegisterRequestHandler<ResolveCollectionTypeRequest, ResolveCollectionTypeHandler>(services);
            RegisterRequestHandler<SendNotificationRequest, SendNotificationHandler>(services);
            RegisterRequestHandler<SendSlackAlertRequest, SendSlackAlertHandler>(services);

            RegisterMediator(services, ServiceLifetime.Transient);

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(MetricsCapturingPipelineBehavior<,>));

            RegisterMediatorBehaviors(services);

            RegisterMediatorExceptionHandlers(services);

            return services;
        }

        public static void RegisterOptions(IServiceCollection services, string configurationBasePath)
        {
            RegisterOption<ExceptionHandlerOptions>(services, $"{configurationBasePath}:{nameof(ExceptionHandlerOptions)}");
            RegisterOption<Collection<CollectionOptions>>(services, $"{configurationBasePath}:{nameof(CollectionOptions)}");
            RegisterOption<RetryProviderOptions>(services, $"{configurationBasePath}:{nameof(RetryProviderOptions)}");
            RegisterOption<HttpApiClientOptions>(services, $"{configurationBasePath}:{nameof(HttpApiClientOptions)}");
            RegisterOption<SlackApiClientOptions>(services, $"{configurationBasePath}:{nameof(SlackApiClientOptions)}");
            RegisterOption<Collection<HttpRequestInterceptorOptions>>(services, $"{configurationBasePath}:{nameof(HttpRequestInterceptorOptions)}");
        }

        public static IServiceCollection RegisterProcessDocumentMediatorHandler<TRequest, THandler>(this IServiceCollection services)
            where THandler : class, IRequestHandler<TRequest, ProcessingStatusResponse>
            where TRequest : ExtractDocumentIdentifierRequest, IRequest<ProcessingStatusResponse>
        {
            services.AddTransient<IRequestHandler<TRequest, ProcessingStatusResponse>, THandler>();

            RegisterExtractProcessDocumentExceptionHandlers<TRequest>(services);

            return services;
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

        internal static IServiceCollection RegisterOption<TOption>(IServiceCollection services, string path)
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
