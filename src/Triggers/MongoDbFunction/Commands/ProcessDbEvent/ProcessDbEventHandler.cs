using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDbFunction.Commands.ProcessDocument;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDbFunction.Commands.ProcessDbEvent
{
    public class ProcessDbEventHandler : IRequestHandler<ProcessDbEventRequest>
    {
        private readonly MongoOptions _options;
        private readonly IMediator _mediator;

        public ProcessDbEventHandler(IOptions<MongoOptions> options, IMediator mediator)
        {
            _options = options.Value;
            _mediator = mediator;
        }

        public Task<Unit> Handle(ProcessDbEventRequest request, CancellationToken cancellationToken)
        {
            var operations = GetOperations(_options.OperationTypes);

            if (!operations.Any(x => x == request.Document.OperationType))
                return Unit.Task;

            return _mediator.Send(new ProcessDocumentRequest
            {
                CollectionName = request.Document.CollectionNamespace.CollectionName,
                Values = request.Document.FullDocument as IDictionary<string, object>
            });
        }

        private static IEnumerable<ChangeStreamOperationType> GetOperations(IEnumerable<string> operationNames)
        {
            var operationTypes = new List<ChangeStreamOperationType>();

            return operationNames.Select(name => Enum.Parse<ChangeStreamOperationType>(name, true));
        }
    }
}
