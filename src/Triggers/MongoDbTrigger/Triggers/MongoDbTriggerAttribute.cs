using Microsoft.Azure.WebJobs.Description;
using System;

namespace MongoDbTrigger.Triggers
{
    [Binding]
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class MongoDbTriggerAttribute : Attribute
    {
    }
}
