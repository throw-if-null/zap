using Microsoft.Azure.WebJobs;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbTrigger;

namespace MongoDbFunction
{
    // https://github.com/Azure/azure-functions-core-tools/issues/2294 - blocked upgrade to .net 5
    public static class Function
    {
        [FunctionName("MongoDbFunction")]
        public static void Run(
            [MongoDbTrigger("test", "items", ConnectionString = "%Connection")]
            ChangeStreamDocument<DemoObject> csd)
        {
        }
    }

    public class DemoObject
    {
        public ObjectId Id { get; set; }

        public string test { get; set; }

        public int number { get; set; }

        public bool yes { get; set; }
    }
}
