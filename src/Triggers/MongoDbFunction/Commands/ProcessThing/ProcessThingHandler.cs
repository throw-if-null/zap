using MediatR;
using MongoDB.Bson;
using MongoDbMonitor.Commands.ProcessDocument;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MongoDbFunction.Commands.ProcessThing
{
    public class ProcessThingHandler : ProcessDocumentHandler<ProcessThingRequest>
    {
        public ProcessThingHandler(IMediator mediator) : base(mediator)
        {
        }

        protected override Task<ObjectId> GetObjectId(IDictionary<string, object> values)
        {
            if (!values.TryGetValue("_id", out var id))
                return Task.FromResult(ObjectId.Empty);

            if (!ObjectId.TryParse(id.ToString(), out var objectId))
                return Task.FromResult(ObjectId.Empty);

            return Task.FromResult(objectId);
        }
    }
}
