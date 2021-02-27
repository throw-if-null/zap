using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDbFunction.Commands.ProcessDocument
{
    public class ProcessDocumentHandler : AsyncRequestHandler<ProcessDocumentRequest>
    {
        private readonly IMediator _mediator;

        public ProcessDocumentHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        protected override async Task Handle(ProcessDocumentRequest request, CancellationToken cancellationToken)
        {
            // Implement Type caching.
            var instance = Activator.CreateInstance(request.AssemblyName, request.HandlerRequestFullQualifiedName)?.Unwrap();

            if (instance == null)
                return;

            var type = instance.GetType();
            var property = type.GetProperty("Values");
            property.SetValue(instance, request.Values);

            var method = typeof(ISender).GetMethods().FirstOrDefault(x => x.Name == "Send" && x.IsGenericMethod == false);

            if (method == null)
                return;

            var task = (Task<object>)method.Invoke(_mediator, new[] { instance, cancellationToken });
            await task;
        }
    }
}
