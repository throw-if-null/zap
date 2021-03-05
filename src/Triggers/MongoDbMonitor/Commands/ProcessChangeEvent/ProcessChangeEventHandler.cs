using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDbMonitor.Commands.Common.Responses;
using MongoDbMonitor.Commands.ResolveCollectionType;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDbMonitor.Commands.ProcessChangeEvent
{
    internal class ProcessChangeEventHandler : IRequestHandler<ProcessChangeEventRequest, ProcessingStatusResponse>
    {
        private readonly Collection<CollectionOptions> _options;
        private readonly IMediator _mediator;

        public ProcessChangeEventHandler(IOptions<Collection<CollectionOptions>> options, IMediator mediator)
        {
            _options = options.Value;
            _mediator = mediator;
        }

        public async Task<ProcessingStatusResponse> Handle(ProcessChangeEventRequest request, CancellationToken cancellationToken)
        {
            var collection =
                _options.First(
                    x =>
                        x.Name.Equals(
                            request.CollectionName,
                            StringComparison.InvariantCultureIgnoreCase));

            var operations = GetOperations(collection.OperationTypes);

            if (!operations.Any(x => x.Equals(request.OperationName, StringComparison.InvariantCultureIgnoreCase)))
                return new ProcessingStatusResponse { FinalStep = ProcessingStep.ProcessChangeEvent };

            var (assemblyName, requestName) = GetRequestName(_options, request.CollectionName);

            var response = await _mediator.Send(
                new ResolveCollectionTypeRequest
                {
                    AssemblyName = assemblyName,
                    HandlerRequestFullQualifiedName = requestName,
                    Values = request.Values
                },
                cancellationToken);

            return response;
        }

        private static (string assemblyName, string requestName) GetRequestName(Collection<CollectionOptions> collections, string collectionName)
        {
            var collection =
                collections
                    .First(x => x.Name.Equals(collectionName, StringComparison.InvariantCultureIgnoreCase));

            return (collection.AssemblyName, collection.HandlerRequestFullQualifiedName);
        }

        private static IEnumerable<string> GetOperations(IEnumerable<string> operationNames)
        {
            return operationNames.Select(name => name.ToLowerInvariant());
        }
    }
}
