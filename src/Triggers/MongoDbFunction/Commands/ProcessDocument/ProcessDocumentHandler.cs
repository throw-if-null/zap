using MediatR;
using Microsoft.Extensions.Options;
using MongoDbFunction.Commands.ProcessItem;
using MongoDbFunction.Commands.ProcessThing;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDbFunction.Commands.ProcessDocument
{
    public class ProcessDocumentHandler : IRequestHandler<ProcessDocumentRequest>
    {
        private readonly MongoOptions _options;
        private readonly IMediator _mediator;

        public ProcessDocumentHandler(IOptions<MongoOptions> options, IMediator mediator)
        {
            _options = options.Value;
            _mediator = mediator;
        }

        public Task<Unit> Handle(ProcessDocumentRequest request, CancellationToken cancellationToken)
        {
            return (request.CollectionName.ToLowerInvariant()) switch
            {
                "items" => _mediator.Send(new ProcessItemRequest { Values = request.Values }, cancellationToken),
                "things" => _mediator.Send(new ProcessThingRequest { Values = request.Values }, cancellationToken),
                _ => Unit.Task,
            };
        }
    }
}
