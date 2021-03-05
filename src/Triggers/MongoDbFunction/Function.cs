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
        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private readonly MonitorRunner _runner;

        public Function(MonitorRunner runner)
        {
            _runner = runner;
        }

        [FunctionName("TestDbMongoFunction")]
        public async Task Run([MongoDbTrigger] ChangeStreamDocument<dynamic> document)
        {
            await _runner.Run(
                document.CollectionNamespace.CollectionName,
                document.OperationType.ToString(),
                document.FullDocument,
                _tokenSource.Token);
        }
    }
}