using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDbMonitor.Commands.ResolveCollectionType;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDbMonitor.Commands.ProcessChangeEvent
{
    internal class ProcessChangeEventHandler : IRequestHandler<ProcessChangeEventRequest, Unit>
    {
        private readonly Collection<CollectionOptions> _options;
        private readonly IMediator _mediator;

        public ProcessChangeEventHandler(IOptions<Collection<CollectionOptions>> options, IMediator mediator)
        {
            _options = options.Value;
            _mediator = mediator;
        }

        public Task<Unit> Handle(ProcessChangeEventRequest request, CancellationToken cancellationToken)
        {
            var collectionName = request.CollectionName;
            var operationType = request.OperationType;

            var collection = _options.First(x => x.Name == collectionName);
            var operations = GetOperations(collection.OperationTypes);

            if (operations.All(x => x != operationType))
                return Unit.Task;

            return
                _mediator.Send(
                    new ResolveCollectionTypeRequest
                    {
                        AssemblyName = collection.AssemblyName,
                        HandlerRequestFullQualifiedName = collection.HandlerRequestFullQualifiedName,
                        Values = request.Values
                    },
                    cancellationToken);
        }

        private static IEnumerable<ChangeStreamOperationType> GetOperations(IEnumerable<string> operationNames)
        {
            var operationTypes = new List<ChangeStreamOperationType>();

            return operationNames.Select(name => Enum.Parse<ChangeStreamOperationType>(name, true));
        }
    }
}
