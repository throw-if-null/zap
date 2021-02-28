using MediatR;
using Microsoft.Azure.WebJobs;
using MongoDB.Driver;
using MongoDbMonitor;
using MongoDbTrigger.Triggers;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDbFunction
{
    // https://github.com/Azure/azure-functions-core-tools/issues/2294 - blocks upgrade to .net 5
    public class Function
    {
        private readonly CancellationTokenSource _cancellationSource = new CancellationTokenSource();

        private readonly DbMonitor _monitor;

        public Function(DbMonitor monitor)
        {
            _monitor = monitor;
        }

        [FunctionName("TestDbMongoFunction")]
        public Task Run([MongoDbTrigger] ChangeStreamDocument<dynamic> document)
        {
            return _monitor.Start(document, _cancellationSource.Token);
        }
    }
}