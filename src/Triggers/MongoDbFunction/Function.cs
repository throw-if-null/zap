using Microsoft.Azure.WebJobs;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbTrigger.Triggers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MongoDbFunction
{
    // https://github.com/Azure/azure-functions-core-tools/issues/2294 - blocked upgrade to .net 5
    public static class Function
    {
        [FunctionName("MongoDbFunction")]
        public static void Run(
            [MongoDbTrigger("test", new []{"items", "things"}, ConnectionString = "%Connection")]
            ChangeStreamDocument<dynamic> document)
        {
            var json = JsonConvert.SerializeObject(document.FullDocument);

            if (document.CollectionNamespace.CollectionName.Equals("items", StringComparison.InvariantCultureIgnoreCase))
            {
                var item = JsonConvert.DeserializeObject<Item>(json);
            }
            else if (document.CollectionNamespace.CollectionName.Equals("things", StringComparison.InvariantCultureIgnoreCase))
            {
                var thing = JsonConvert.DeserializeObject<Thing>(json);
            }
        }
    }

    public class BaseObject
    {
        [JsonProperty("_id")]
        public ObjectId Id { get; set; }
    }

    public class Item : BaseObject
    {
        public string Test { get; set; }

        public int Number { get; set; }

        public bool Yes { get; set; }

        public IEnumerable<string> Children { get; set; }
    }

    public class Thing : BaseObject
    {
        public int Number { get; set; }

        public int Number2 { get; set; }
    }
}
