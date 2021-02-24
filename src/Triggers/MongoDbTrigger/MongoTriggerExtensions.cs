using Microsoft.Azure.WebJobs;
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

            builder.Services.AddSingleton<MongoDbCollectionFactory>();

            builder.AddExtension<MongoDbExtensionsProvider>();

            return builder;
        }
    }
}
