using MediatR;
using MongoDbMonitor.Commands.ProcessDocument;

namespace MongoDbFunction.Commands.ProcessThing
{
    public class ProcessThingHandler : ProcessDocumentHandler<ProcessThingRequest>
    {
        public ProcessThingHandler(IMediator mediator) : base(mediator)
        {
        }

        protected override string PropertyToExtract => "_id";
    }
}
