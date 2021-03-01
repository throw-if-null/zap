using MediatR;
using Microsoft.Extensions.Caching.Memory;
using MongoDbMonitor.Commands.Exceptions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDbMonitor.Commands.ResolveCollectionType
{
    internal class ResolveCollectionTypeHandler : IRequestHandler<ResolveCollectionTypeRequest, Unit>
    {
        private const string VALUES_PROPERTY_NAME = "Values";
        private const string SEND_METHOD_NAME = "Send";

        private readonly IMediator _mediator;
        private readonly IMemoryCache _cache;

        public ResolveCollectionTypeHandler(IMediator mediator, IMemoryCache cache)
        {
            _mediator = mediator;
            _cache = cache;
        }

        public async Task<Unit> Handle(ResolveCollectionTypeRequest request, CancellationToken cancellationToken)
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

        private static object CreateInstance(ResolveCollectionTypeRequest request)
        {
            var instance = Activator.CreateInstance(request.AssemblyName, request.HandlerRequestFullQualifiedName)?.Unwrap();

            if (instance == null)
                throw new InvalidRequestTypeException(request.AssemblyName, request.HandlerRequestFullQualifiedName);

            var type = instance.GetType();

            var valuesProperty = type.GetProperty(VALUES_PROPERTY_NAME);

            if (valuesProperty == null)
                throw new MissingRequiredPropertyException(request.HandlerRequestFullQualifiedName, VALUES_PROPERTY_NAME);

            valuesProperty.SetValue(instance, request.Values);

            return instance;
        }
    }
}
