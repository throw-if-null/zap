using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
using Moq;
using Moq.Protected;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using static MongoDbMonitor.WebJobExtensions;

namespace MongoDbMonitorTest
{
    internal static class TestServiceFactory
    {
        internal static HttpClient GetMockHttpClient()
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(() => new HttpResponseMessage(HttpStatusCode.OK));

            return new HttpClient(mockHttpMessageHandler.Object);
        }

        public static IServiceCollection RegisterServices(
            bool includeExceptionHandler = false,
            bool mockHttpClients = false,
            string jsonSettingsName = "test.json")
        {
            var services = new ServiceCollection();

            services.AddSingleton(BuildConfiguration(jsonSettingsName));

            RegisterOptions<Collection<CollectionOptions>>(services, "MonitorOptions:CollectionOptions");
            RegisterOptions<RetryProviderOptions>(services, "MonitorOptions:RetryProviderOptions");
            RegisterOptions<HttpApiClientOptions>(services, "MonitorOptions:HttpApiClientOptions");
            RegisterOptions<SlackApiClientOptions>(services, "MonitorOptions:SlackApiClientOptions");

            services.AddLogging(x => x.AddConsole());

            services.AddMemoryCache();

            if (mockHttpClients)
            {
                services.AddTransient<IHttpApiClient>(x => new HttpApiClient(
                    x.GetRequiredService<IOptions<HttpApiClientOptions>>(),
                    GetMockHttpClient(),
                    x.GetRequiredService<IRetryProvider>()));

                services.AddTransient<ISlackApiClient>(x => new SlackApiClient(
                    x.GetRequiredService<IOptions<SlackApiClientOptions>>(),
                    GetMockHttpClient(),
                    x.GetRequiredService<IRetryProvider>()));
            }
            else
            {
                services.AddHttpClient<IHttpApiClient, HttpApiClient>(x => GetMockHttpClient());
                services.AddHttpClient<ISlackApiClient, SlackApiClient>(x => GetMockHttpClient());
            }

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
