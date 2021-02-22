using Microsoft.Azure.WebJobs.Description;
using System;

namespace MongoDbTrigger.Triggers
{
    [Binding]
    [AttributeUsage(AttributeTargets.Parameter)]
    public class MongoDbTriggerAttribute : Attribute
    {

        public string Database { get; }

        public string Collection { get; }

        [AppSetting]
        public string ConnectionString { get; set; }

        public MongoDbTriggerAttribute(string database, string collection)
        {
            Database = database;
            Collection = collection;
        }
    }
}
