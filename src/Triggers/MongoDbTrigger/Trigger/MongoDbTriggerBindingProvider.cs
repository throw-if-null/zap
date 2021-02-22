﻿using Microsoft.Azure.WebJobs.Host.Triggers;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MongoDbTrigger
{
    public class MongoDbTriggerBindingProvider : ITriggerBindingProvider
    {
        private readonly IConfiguration _configuration;

        public MongoDbTriggerBindingProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<ITriggerBinding> TryCreateAsync(TriggerBindingProviderContext context)
        {
            var parameter = context?.Parameter ?? throw new ArgumentNullException(nameof(context));
            if (parameter.ParameterType.GetGenericTypeDefinition() != typeof(ChangeStreamDocument<>))
                return null;

            var attr = parameter.GetCustomAttribute<MongoDbTriggerAttribute>(inherit: false);
            if (attr == null)
                return null;

            var triggerConnectionString = ResolveAttributeConnectionString(attr);

            var genericType = parameter.ParameterType.GetGenericArguments().First();

            return Task.FromResult<ITriggerBinding>(new MongoDbTriggerBinding(
                genericType,
                attr.Database,
                attr.Collection,
                attr.ConnectionString));
        }

        private string ResolveAttributeConnectionString(MongoDbTriggerAttribute triggerAttribute) =>
            triggerAttribute.ConnectionString.Contains("%")
                ? ResolveConnectionString(
                    triggerAttribute.ConnectionString.Replace("%", ""),
                    nameof(MongoDbTriggerAttribute.ConnectionString))
                : triggerAttribute.ConnectionString;

        private string ResolveConnectionString(string unresolvedConnectionString, string propertyName)
        {
            if (string.IsNullOrEmpty(unresolvedConnectionString)) return null;

            var resolvedString = _configuration.GetConnectionStringOrSetting(unresolvedConnectionString);

            if (string.IsNullOrEmpty(resolvedString))
                throw new InvalidOperationException(
                    $"Unable to resolve app setting for property '{nameof(MongoDbTriggerAttribute)}.{propertyName}'. Make sure the app setting exists and has a valid value.");

            return resolvedString;
        }
    }
}
