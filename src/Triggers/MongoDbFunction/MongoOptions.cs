using System.Collections.ObjectModel;

namespace MongoDbFunction
{
    public class MongoOptions
    {
        public string ConnectionString { get; set; }

        public string Database { get; set; }

        public Collection<string> Collections { get; set; }

        public Collection<string> OperationTypes { get; set; }
    }
}
