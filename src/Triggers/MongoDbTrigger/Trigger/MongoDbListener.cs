using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Listeners;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDbTrigger
{
    public class MongoDbListener : IListener
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private readonly Type _genericType;
        private readonly ITriggeredFunctionExecutor _executor;
        private readonly string _connectionString;
        private readonly string _databaseName;
        private readonly string _collectionName;

        //private Task _watchTask;
        private bool _disposedValue;

        public MongoDbListener(
            Type genericType,
            string databaseName,
            string collectionName,
            string connectionString,
            ITriggeredFunctionExecutor executor)
        {
            _genericType = genericType;
            _databaseName = databaseName;
            _collectionName = collectionName;
            _connectionString = connectionString;
            _executor = executor;
        }

        public void Cancel()
        {
            _cancellationTokenSource.Cancel();
        }

        public Task StartAsync(CancellationToken cancellationToken) =>
            WatchAsync(MongoUrl.Create(_connectionString), _cancellationTokenSource.Token);

        private async Task WatchAsync(MongoUrl url, CancellationToken cancellationToken)
        {
            var db = new MongoClient(_connectionString).GetDatabase(_databaseName);


            var method = typeof(IMongoDatabase).GetMethod(nameof(IMongoDatabase.GetCollection));

            dynamic collection =
                method
                    .MakeGenericMethod(_genericType)
                    .Invoke(db, new object[] { _collectionName, new MongoCollectionSettings() });

            await Watch(collection, cancellationToken);
        }

        private async Task Watch<T>(IMongoCollection<T> collection, CancellationToken cancellationToken)
        {
            var cursor = await collection.WatchAsync(cancellationToken: cancellationToken);
            await cursor.ForEachAsync(document => WatchChange(document, cancellationToken), cancellationToken);
        }

        private async Task WatchChange(BsonDocumentBackedClass arg, CancellationToken cancellationToken)
        {
            var input = new TriggeredFunctionData
            {
                TriggerValue = arg
            };

            try
            {
                await _executor.TryExecuteAsync(input, cancellationToken);
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
