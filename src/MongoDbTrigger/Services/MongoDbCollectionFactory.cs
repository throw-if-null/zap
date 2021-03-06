using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace MongoDbTrigger.Services
{
    internal class MongoDbCollectionFactory
    {
        private static readonly Func<MongoDbTriggerOptions, IMongoClient> GetClient = delegate (MongoDbTriggerOptions options)
        {
            return new MongoClient(options.ConnectionString);
        };

        private static readonly Func<MongoDbTriggerOptions, IMongoDatabase> GetDatabase = delegate (MongoDbTriggerOptions options)
        {
            return GetClient(options).GetDatabase(options.Database);
        };

        private static readonly Func<MongoDbTriggerOptions, IEnumerable<IMongoCollection<dynamic>>> GetColections = delegate (MongoDbTriggerOptions options)
        {
            var collections = new List<IMongoCollection<dynamic>>();

            foreach(var collectionName in options.Collections)
            {
                collections.Add(GetDatabase(options).GetCollection<dynamic>(collectionName));
            }

            return collections;
        };

        private readonly MongoDbTriggerOptions _options;

        public MongoDbCollectionFactory(IOptions<MongoDbTriggerOptions> options)
        {
            _options = options.Value;
        }

        public IEnumerable<IMongoCollection<dynamic>> GetMongoCollection()
        {
            var collections = GetColections(_options);

            return collections;
        }
    }
}
