using MediatR;
using MongoDbMonitor.Commands.ExtractDocumentIdentifier;

namespace MongoDbFunction.Commands.ProcessItem
{
    public class ProcessItemHandler : ExtractDocumentIdentifierHandler<ProcessItemRequest>
    {
        public ProcessItemHandler(IMediator mediator) : base(mediator)
        {
        }
    }
}
