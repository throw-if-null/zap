using Microsoft.Azure.WebJobs.Description;
using System;

namespace MongoDbTrigger.Triggers
{
    [Binding]
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class MongoDbTriggerAttribute : Attribute
    {

        public string Database { get; }

        public string[] Collections { get; }

        [AppSetting]
        public string ConnectionString { get; set; }

        public MongoDbTriggerAttribute(string database, string[] collections)
        {
            Database = database;
            Collections = collections;
        }
    }
}
