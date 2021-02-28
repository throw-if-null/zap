using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDbMonitor.Commands.Common;
using MongoDbMonitor.Commands.ResolveCollection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDbMonitor.Commands.ProcessChangeEvent
{
    internal class ProcessChangeEventHandler : IErrorHandlingRequestHanlder<ProcessChangeEventRequest, Unit>
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
            var document = request.Document;
            var collectionName = document.CollectionNamespace.CollectionName;
            var operationType = document.OperationType;

            var collection = _options.First(x => x.Name == collectionName);
            var operations = GetOperations(collection.OperationTypes);

            if (operations.All(x => x != operationType))
                return Unit.Task;

            return
                _mediator.Send(
                    new ResolveCollectionRequest
                    {
                        CollectionName = document.CollectionNamespace.CollectionName,
                        AssemblyName = collection.AssemblyName,
                        HandlerRequestFullQualifiedName = collection.HandlerRequestFullQualifiedName,
                        Values = document.FullDocument as IDictionary<string, object>
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
