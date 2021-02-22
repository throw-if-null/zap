using MongoDB.Driver;

namespace MongoDbTrigger
{
    public interface IMongoDbConnectionFactory
    {
        IMongoCollection<T> GetMongoCollection<T>(string name);
    }

    public class MongoDbConnectionFactory : IMongoDbConnectionFactory
    {
        private IMongoClient _client;

        public MongoDbConnectionFactory(string connectionString)
        {
            _client = new MongoClient(connectionString);
        }

        public IMongoCollection<T> GetMongoCollection<T>(string name)
        {
            var collection = _client.GetDatabase("test").GetCollection<T>(name);

            return collection;
        }
    }
}
