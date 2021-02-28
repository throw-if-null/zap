using MediatR;
using Microsoft.Extensions.Caching.Memory;
using MongoDbMonitor.Commands.Common;
using MongoDbMonitor.Commands.Exceptions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDbMonitor.Commands.ResolveCollection
{
    internal class ResolveCollectionHandler : IErrorHandlingRequestHanlder<ResolveCollectionRequest, Unit>
    {
        private const string VALUES_PROPERTY_NAME = "Values";
        private const string COLLECTION_NAME_PROPERTY_NAME = "CollectionName";
        private const string SEND_METHOD_NAME = "Send";

        private readonly IMediator _mediator;
        private readonly IMemoryCache _cache;

        public ResolveCollectionHandler(IMediator mediator, IMemoryCache cache)
        {
            _mediator = mediator;
            _cache = cache;
        }

        public async Task<Unit> Handle(ResolveCollectionRequest request, CancellationToken cancellationToken)
        {
            var key = $"{request.AssemblyName}-{request.HandlerRequestFullQualifiedName}";

            object instance = _cache.Get(key);

            if (instance == null)
                instance = _cache.Set(key, CreateInstance(request));

            dynamic method =
                typeof(ISender)
                    .GetMethods()
                    .First(
                        x =>
                            x.Name == SEND_METHOD_NAME &&
                            x.IsGenericMethod == false &&
                            x.ReturnType == typeof(Task<object>));

            _ = await method.Invoke(_mediator, new[] { instance, cancellationToken });

            return Unit.Value;
        }

        private static object CreateInstance(ResolveCollectionRequest request)
        {
            var instance = Activator.CreateInstance(request.AssemblyName, request.HandlerRequestFullQualifiedName)?.Unwrap();

            if (instance == null)
                throw new InvalidRequestTypeException(request.AssemblyName, request.HandlerRequestFullQualifiedName);

            var type = instance.GetType();

            var valuesProperty = type.GetProperty(VALUES_PROPERTY_NAME);

            if (valuesProperty == null)
                throw new MissingRequiredPropertyException(request.HandlerRequestFullQualifiedName, VALUES_PROPERTY_NAME);

            valuesProperty.SetValue(instance, request.Values);

            var collectionNameProperty = type.GetProperty(COLLECTION_NAME_PROPERTY_NAME);

            if (collectionNameProperty == null)
                throw new MissingRequiredPropertyException(request.HandlerRequestFullQualifiedName, COLLECTION_NAME_PROPERTY_NAME);

            collectionNameProperty.SetValue(instance, request.CollectionName);

            return instance;
        }
    }
}
