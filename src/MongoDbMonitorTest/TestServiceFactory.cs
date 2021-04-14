using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDbFunction.Commands.ProcessItem;
using MongoDbFunction.Commands.ProcessThing;
using MongoDbMonitor;
using MongoDbMonitor.Clients.HttpApi;
using MongoDbMonitor.Clients.SlackApi;
using MongoDbMonitor.Commands.Common;
using MongoDbMonitor.Commands.Common.Responses;
using MongoDbMonitor.Commands.ProcessChangeEvent;
using MongoDbMonitor.Commands.ResolveCollectionType;
using MongoDbMonitor.Commands.SendNotification;
using MongoDbMonitor.Commands.SendSlackAlert;
using MongoDbMonitor.CrossCutting.QoS;
using Scissors;
using System.Collections.ObjectModel;
using System.IO;

using static MongoDbMonitor.WebJobExtensions;

namespace MongoDbMonitorTest
{
    internal static class TestServiceFactory
    {
        private const string ConfigurationBasePath = "MonitorOptions";

        public static IServiceCollection RegisterServices(
            bool includeExceptionHandler = false,
            string jsonSettingsName = "test.json")
        {
            var services = new ServiceCollection();

            services.AddSingleton(BuildConfiguration(jsonSettingsName));

            RegisterOptions<Collection<CollectionOptions>>(services, $"{ConfigurationBasePath}:{nameof(CollectionOptions)}");
            RegisterOptions<RetryProviderOptions>(services, $"{ConfigurationBasePath}:{nameof(RetryProviderOptions)}");
            RegisterOptions<HttpApiClientOptions>(services, $"{ConfigurationBasePath}:{nameof(HttpApiClientOptions)}");
            RegisterOptions<SlackApiClientOptions>(services, $"{ConfigurationBasePath}:{nameof(SlackApiClientOptions)}");
            RegisterOptions<Collection<HttpRequestInterceptorOptions>>(services, $"{ConfigurationBasePath}:{nameof(HttpRequestInterceptorOptions)}");

            services.AddLogging(x => x.AddConsole());

            services.AddMemoryCache();

            services.AddTransient<HttpRequestInterceptor>();

            services.AddHttpClient<IHttpApiClient, HttpApiClient>().AddHttpMessageHandler<HttpRequestInterceptor>();
            services.AddHttpClient<ISlackApiClient, SlackApiClient>().AddHttpMessageHandler<HttpRequestInterceptor>();

            services.AddSingleton<IRetryProvider, RetryProvider>();

            services.AddTransient<MonitorRunner>();

            services.AddTransient<IRequestHandler<ProcessChangeEventRequest, ProcessingStatusResponse>, ProcessChangeEventHandler>();
            services.AddTransient<IRequestHandler<ResolveCollectionTypeRequest, ProcessingStatusResponse>, ResolveCollectionTypeHandler>();
            services.AddTransient<IRequestHandler<SendNotificationRequest, ProcessingStatusResponse>, SendNotificationHandler>();
            services.AddTransient<IRequestHandler<SendSlackAlertRequest, ProcessingStatusResponse>, SendSlackAlertHandler>();
            services.AddTransient<IRequestHandler<ProcessItemRequest, ProcessingStatusResponse>, ProcessItemHandler>();
            services.AddTransient<IRequestHandler<ProcessThingRequest, ProcessingStatusResponse>, ProcessThingHandler>();

            RegisterMediator(services, ServiceLifetime.Transient);

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(MetricsCapturingPipelineBehavior<,>));

            RegisterMediatorBehaviors(services);

            if (includeExceptionHandler)
            {
                RegisterExtractProcessDocumentExceptionHandlers<ProcessItemRequest>(services);
                RegisterExtractProcessDocumentExceptionHandlers<ProcessThingRequest>(services);

                RegisterMediatorExceptionHandlers(services);
            }

            return services;
        }

        private static IConfiguration BuildConfiguration(string jsonSettingsName = "test.json")
        {
            var builder = new ConfigurationBuilder();

            builder.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), jsonSettingsName), false);

            return builder.Build();
        }
    }
}
