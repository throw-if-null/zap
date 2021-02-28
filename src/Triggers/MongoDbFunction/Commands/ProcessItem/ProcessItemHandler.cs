using MediatR;
using MongoDB.Bson;
using MongoDbMonitor.Commands.ProcessDocument;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MongoDbFunction.Commands.ProcessItem
{
    public class ProcessItemHandler : ProcessDocumentHandler<ProcessItemRequest>
    {
        public ProcessItemHandler(IMediator mediator) : base(mediator)
        {
        }

        protected override Task<ObjectId> GetObjectId(IDictionary<string, object> values)
        {
            if (!values.TryGetValue("_id", out var value))
                return Task.FromResult(ObjectId.Empty);

            if (!ObjectId.TryParse(value.ToString(), out var id))
                return Task.FromResult(ObjectId.Empty);

            return Task.FromResult(id);
        }
    }
}
