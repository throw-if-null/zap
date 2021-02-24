using Microsoft.Azure.WebJobs.Host.Triggers;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MongoDbTrigger.Triggers;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MongoDbTrigger.Bindings
{
    internal sealed class MongoDbBindingProvider : ITriggerBindingProvider
    {
        private readonly IConfiguration _configuration;

        public MongoDbBindingProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<ITriggerBinding> TryCreateAsync(TriggerBindingProviderContext context) => Task.FromResult(TryCreate(context));

        public ITriggerBinding TryCreate(TriggerBindingProviderContext context)
        {
            var parameter = context?.Parameter ?? throw new ArgumentNullException(nameof(context));

            if (parameter.ParameterType.GetGenericTypeDefinition() != typeof(ChangeStreamDocument<>))
                return null;

            var attribute = parameter.GetCustomAttribute<MongoDbTriggerAttribute>(inherit: false);

            if (attribute == null)
                return null;

            var database = ResolveDatabase();
            var collections = ResolveCollections();
            var connectionString = ResolveConnectionString();

            return new MongoDbTriggerBinding(database, collections, connectionString);
        }

        private string ResolveDatabase()
        {
            string configPath = $"AzureFunctionsJobHost:MongoDatabase";

            var value = _configuration.GetSection(configPath).Get<string>();

            if (string.IsNullOrEmpty(value))
                throw new ArgumentException($"Unable to configuration key: '{configPath}'.");

            return value;
        }

        private string ResolveConnectionString()
        {
            var configPath = $"AzureFunctionsJobHost:MongoConnectionString";

            var value = _configuration.GetSection(configPath).Get<string>();

            if (string.IsNullOrEmpty(value))
                throw new ArgumentException($"Unable to configuration key: '{configPath}'.");

            return value;
        }

        private string[] ResolveCollections()
        {
            var configPath = $"AzureFunctionsJobHost:MongoCollections";
            var value = _configuration.GetSection(configPath).Get<string[]>();

            if (value == null || value.Length == 0)
                throw new ArgumentException($"Unable to configuration key: '{configPath}'.");

            return value;
        }
    }
}
