using MediatR;
using MongoDbMonitor.Commands.Common;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDbMonitor.Commands.ResolveCollection
{
    internal class ResolveCollectionHandler : IErrorHandlingRequestHanlder<ResolveCollectionRequest, Unit>
    {
        private readonly IMediator _mediator;

        public ResolveCollectionHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Unit> Handle(ResolveCollectionRequest request, CancellationToken cancellationToken)
        {
            // Implement Type caching.
            var instance = Activator.CreateInstance(request.AssemblyName, request.HandlerRequestFullQualifiedName)?.Unwrap();

            if (instance == null)
                return Unit.Value;

            var type = instance.GetType();

            var valuesProperty = type.GetProperty("Values");
            valuesProperty.SetValue(instance, request.Values);

            var collectionNameProperty = type.GetProperty("CollectionName");
            collectionNameProperty.SetValue(instance, request.CollectionName);

            dynamic method = typeof(ISender).GetMethods().FirstOrDefault(x => x.Name == "Send" && x.IsGenericMethod == false);

            if (method == null)
                return Unit.Value;

            if (method.ReturnType != typeof(Task<object>))
                return Unit.Value;

            _ = await method.Invoke(_mediator, new[] { instance, cancellationToken });

            return Unit.Value;
        }
    }
}
