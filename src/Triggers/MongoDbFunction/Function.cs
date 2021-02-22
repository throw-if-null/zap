using Microsoft.Azure.WebJobs;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDbTrigger.Triggers;
using Newtonsoft.Json;
using System;

namespace MongoDbFunction
{
    public class Handler
    {
        public void Handle(ChangeStreamDocument<dynamic> document)
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
    // https://github.com/Azure/azure-functions-core-tools/issues/2294 - blocked upgrade to .net 5
    public static class Function
    {
        [FunctionName("MongoDbFunction")]
        public static void Run(
            [MongoDbTrigger("test", new []{"items", "things"}, ConnectionString = "%Connection")]
            ChangeStreamDocument<dynamic> csd)
        {
            new Handler().Handle(csd);
        }
    }


    public class BaseObject
    {
        public string _id { get; set; }
    }

    public class Item : BaseObject
    {
        public string test { get; set; }

        public int number { get; set; }

        public bool yes { get; set; }
    }

    public class Thing : BaseObject
    {
        public int number { get; set; }

        public int number2 { get; set; }
    }
}
