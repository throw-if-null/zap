using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDbFunction.Commands.ProcessDocument;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDbFunction.Commands.ProcessDbEvent
{
    public class ProcessDbEventHandler : IRequestHandler<ProcessDbEventRequest>
    {
        private readonly Collection<CollectionOptions> _options;
        private readonly IMediator _mediator;

        public ProcessDbEventHandler(IOptions<Collection<CollectionOptions>> options, IMediator mediator)
        {
            _options = options.Value;
            _mediator = mediator;
        }

        public Task<Unit> Handle(ProcessDbEventRequest request, CancellationToken cancellationToken)
        {
            var collection = _options.First(x => x.Name == request.Document.CollectionNamespace.CollectionName);

            var operations = GetOperations(collection.OperationTypes);

            if (!operations.Any(x => x == request.Document.OperationType))
                return Unit.Task;

            return _mediator.Send(new ProcessDocumentRequest
            {
                CollectionName = request.Document.CollectionNamespace.CollectionName,
                AssemblyName = collection.AssemblyName,
                HandlerRequestFullQualifiedName = collection.HandlerRequestFullQualifiedName,
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
