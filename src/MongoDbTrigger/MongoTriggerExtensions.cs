using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDbTrigger.Extensions;
using MongoDbTrigger.Services;
using System;

namespace MongoDbTrigger
{
    public static class MongoTriggerExtensions
    {
        /// <summary>
        /// Adds the MongoDbTrigger extension to the provided <see cref="IWebJobsBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IWebJobsBuilder"/> to configure.</param>
        public static IWebJobsBuilder AddMongoDbTrigger(this IWebJobsBuilder builder)
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));

            builder.Services
                .AddOptions<MongoDbTriggerOptions>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    settings.ConnectionString = configuration.GetSection("AzureFunctionsJobHost:MongoOptions:ConnectionString").Get<string>();
                    settings.Database = configuration.GetSection("AzureFunctionsJobHost:MongoOptions:Database").Get<string>();

                    int index = 0;

                    while(true)
                    {
                        var collectionName = configuration.GetSection("AzureFunctionsJobHost:MongoOptions:CollectionOptions").GetSection($"{index}:Name").Get<string>();

                        if (string.IsNullOrWhiteSpace(collectionName))
                            break;

                        settings.Collections.Add(collectionName);

                        index++;
                    }

                    configuration.Bind(settings);
                });

            builder.Services.AddSingleton<MongoDbCollectionFactory>();

            builder.AddExtension<MongoDbExtensionsProvider>();

            return builder;
        }
    }
}
