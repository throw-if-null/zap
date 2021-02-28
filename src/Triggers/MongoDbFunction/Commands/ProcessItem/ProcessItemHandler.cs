using MediatR;
using MongoDbMonitor.Commands.ProcessDocument;

namespace MongoDbFunction.Commands.ProcessItem
{
    public class ProcessItemHandler : ProcessDocumentHandler<ProcessItemRequest>
    {
        public ProcessItemHandler(IMediator mediator) : base(mediator)
        {
        }

        protected override string PropertyToExtract => "_id";
    }
}
