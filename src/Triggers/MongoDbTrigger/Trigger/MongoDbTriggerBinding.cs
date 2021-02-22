using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.Azure.WebJobs.Host.Triggers;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MongoDbTrigger
{
    public class MongoDbTriggerBinding : ITriggerBinding
    {
        private readonly Type _genericType;
        private readonly string _database;
        private readonly string _collection;
        private readonly string _connectionString;

        public Type TriggerValueType => typeof(ChangeStreamDocument<>).MakeGenericType(_genericType);

        public IReadOnlyDictionary<string, Type> BindingDataContract { get; } = new Dictionary<string, Type>();

        public MongoDbTriggerBinding(
            Type genericType,
            string database,
            string collection,
            string connectionString)
        {
            _genericType = genericType;
            _database = database;
            _collection = collection;
            _connectionString = connectionString;
        }

        public Task<ITriggerData> BindAsync(object value, ValueBindingContext context)
        {
            ITriggerData triggerData =
                new TriggerData(
                    new ValueProvider(value, _genericType),
                    new Dictionary<string, object>());

            return Task.FromResult(triggerData);
        }

        public Task<IListener> CreateListenerAsync(ListenerFactoryContext context)
        {
            return Task.FromResult<IListener>(
                new MongoDbListener(
                    _genericType,
                    _database,
                    _collection,
                    _connectionString,
                    context.Executor));
        }

        public ParameterDescriptor ToParameterDescriptor()
        {
            return new MongoDbTriggerParameterDescriptor
            {
                DatabaseName = _database,
                Collectionname = _collection
            };
        }

        private class ValueProvider : IValueProvider
        {
            private readonly object _value;

            public ValueProvider(object value, Type triggerValueType)
            {
                _value = value;
                Type = typeof(ChangeStreamDocument<>).MakeGenericType(triggerValueType);
            }

            public Type Type { get; }

            public Task<object> GetValueAsync() => Task.FromResult(_value);

            public string ToInvokeString() => DateTime.Now.ToString("o");
        }
    }
}
