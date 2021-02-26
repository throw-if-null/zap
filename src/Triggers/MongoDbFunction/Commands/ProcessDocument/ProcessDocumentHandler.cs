using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDbFunction.Commands.ProcessDocument
{
    public class ProcessDocumentHandler : AsyncRequestHandler<ProcessDocumentRequest>
    {
        private readonly MongoOptions _options;
        private readonly IMediator _mediator;

        public ProcessDocumentHandler(IOptions<MongoOptions> options, IMediator mediator)
        {
            _options = options.Value;
            _mediator = mediator;
        }

        protected override async Task Handle(ProcessDocumentRequest request, CancellationToken cancellationToken)
        {
            Type type = Type.GetType($"MongoDbFunction.Commands.{request.HandlerNamespace}.{request.HandlerNamespace}Request");

            if (type == null)
                return;

            // Implement Type caching.
            var instance = Activator.CreateInstance(type);
            var property = type.GetProperty("Values");
            property.SetValue(instance, request.Values);

            var method = typeof(ISender).GetMethods().FirstOrDefault(x => x.Name == "Send" && x.IsGenericMethod == false);

            if (method == null)
                return;

            //var send = method.MakeGenericMethod(type);


            var task = (Task<object>)method.Invoke(_mediator, new[] { instance, cancellationToken });
            await task;
        }
    }
}
