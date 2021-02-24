using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Listeners;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDbTrigger.Listeners
{
    internal class MongoDbListener : IListener
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private readonly ITriggeredFunctionExecutor _executor;
        private readonly string _connectionString;
        private readonly string _database;
        private readonly string[] _collections;

        private bool _disposedValue;

        public MongoDbListener(
            string database,
            string[] collections,
            string connectionString,
            ITriggeredFunctionExecutor executor)
        {
            _database = database;
            _collections = collections;
            _connectionString = connectionString;
            _executor = executor;
        }

        public void Cancel()
        {
            _cancellationTokenSource.Cancel();
        }

        public Task StartAsync(CancellationToken cancellationToken) => WatchAsync(_cancellationTokenSource.Token);

        private async Task WatchAsync(CancellationToken cancellationToken)
        {
            var database = new MongoClient(_connectionString).GetDatabase(_database);

            var childCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            var tasks = new List<Task>(_collections.Length);

            foreach (var collectionName in _collections)
            {
                var collection = database.GetCollection<dynamic>(collectionName);

                var task = Watch(collection, childCancellation.Token);

                tasks.Add(task);
            }

            await Task.WhenAll(tasks);
        }

        private async Task Watch(IMongoCollection<dynamic> collection, CancellationToken cancellation)
        {
            var cursor = await collection.WatchAsync(null, cancellation);
            await cursor.ForEachAsync(document => WatchChange(document, cancellation), cancellation);
        }

        private async Task WatchChange(BsonDocumentBackedClass document, CancellationToken cancellation)
        {
            var input = new TriggeredFunctionData
            {
                TriggerValue = document
            };

            try
            {
                await _executor.TryExecuteAsync(input, cancellation);
            }
            catch
            {
                // We don't want any function errors to stop the execution
                // schedule. Errors will be logged to Dashboard already.
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _cancellationTokenSource.Cancel();
                    _cancellationTokenSource.Dispose();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
