using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDbWatcher
{
    public class Item
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string test { get; set; }

        public int number { get; set; }

        public bool yes { get; set; }
    }

    internal class Program
    {
        static async Task Main(string[] args)
        {
            var client = new MongoClient("mongodb+srv://testUser:testUser@testcluster.okicx.mongodb.net/myFirstDatabase?retryWrites=true&w=majority");

            var collection = client.GetDatabase("test").GetCollection<Item>("items");

            var document = await collection.FindAsync<Item>(
                new FilterDefinitionBuilder<Item>().Empty,
                null,
                CancellationToken.None);

            var pipeline =
                new EmptyPipelineDefinition<ChangeStreamDocument<Item>>()
                .Match(x =>
                    x.OperationType == ChangeStreamOperationType.Update ||
                    x.OperationType == ChangeStreamOperationType.Replace);

            var options = new ChangeStreamOptions
            {
                FullDocument = ChangeStreamFullDocumentOption.UpdateLookup,
            };

            using var cursor = await collection.WatchAsync(pipeline, options, CancellationToken.None);

            foreach (ChangeStreamDocument<Item> change in cursor.ToEnumerable())
            {
                ChangeStreamOperationType operationType = change.OperationType;
                // process change event
            }
        }
    }
}
