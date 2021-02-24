using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace MongoDbTrigger.Services
{
    internal class MongoDbCollectionFactory
    {
        private static readonly Func<IConfiguration, IMongoClient> GetClient = delegate (IConfiguration configuration)
        {
            return new MongoClient(configuration.GetConnectionString());
        };

        private static readonly Func<IConfiguration, IMongoDatabase> GetDatabase = delegate (IConfiguration configuration)
        {
            return GetClient(configuration).GetDatabase(configuration.GetDatabaseName());
        };

        private static readonly Func<IConfiguration, IEnumerable<IMongoCollection<dynamic>>> GetColections = delegate (IConfiguration configuration)
        {
            var collections = new List<IMongoCollection<dynamic>>();

            foreach(var collectionName in configuration.GetCollectionNames())
            {
                collections.Add(GetDatabase(configuration).GetCollection<dynamic>(collectionName));
            }

            return collections;
        };

        private readonly IConfiguration _configuration;

        public MongoDbCollectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IEnumerable<IMongoCollection<dynamic>> GetMongoCollection()
        {
            var collections = GetColections(_configuration);

            return collections;
        }
    }

    internal static class ConfigurationExtensions
    {
        public static string GetDatabaseName(this IConfiguration configuration)
        {
            return configuration.GetValue<string>("AzureFunctionsJobHost:MongoDatabase");
        }

        public static string GetConnectionString(this IConfiguration configuration)
        {
            return configuration.GetValue<string>("AzureFunctionsJobHost:MongoConnectionString");
        }

        public static string[] GetCollectionNames(this IConfiguration configuration)
        {
            return configuration.GetValue<string[]>("AzureFunctionsJobHost:MongoCollections");
        }

        private static T GetValue<T>(this IConfiguration configuration, string configPath)
        {
            return configuration.GetSection(configPath).Get<T>();
        }
    }
}
