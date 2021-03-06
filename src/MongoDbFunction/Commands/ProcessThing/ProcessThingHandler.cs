using MediatR;
using MongoDbMonitor.Commands.ExtractDocumentIdentifier;

namespace MongoDbFunction.Commands.ProcessThing
{
    public class ProcessThingHandler : ExtractDocumentIdentifierHandler<ProcessThingRequest>
    {
        public ProcessThingHandler(IMediator mediator) : base(mediator)
        {
        }
    }
}
