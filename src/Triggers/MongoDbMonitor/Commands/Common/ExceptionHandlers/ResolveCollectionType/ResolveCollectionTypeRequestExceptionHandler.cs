using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using MongoDbMonitor.Commands.ResolveCollectionType;
using MongoDbMonitor.Commands.SendSlackAlert;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDbMonitor.Commands.Common.ExceptionHandlers.ResolveCollectionType
{
    internal abstract class ResolveCollectionTypeRequestExceptionHandler<TException> :
        IRequestExceptionHandler<ResolveCollectionTypeRequest, Unit, TException>
        where TException : Exception
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        protected ResolveCollectionTypeRequestExceptionHandler(
            IMediator mediator,
            ILogger<ResolveCollectionTypeRequestExceptionHandler<TException>> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Handle(
            ResolveCollectionTypeRequest request,
            TException exception,
            RequestExceptionHandlerState<Unit> state,
            CancellationToken cancellationToken)
        {
            _logger.LogError(exception, exception.Message);

            var response =
                await
                    _mediator.Send(new SendSlackAlertRequest
                    {
                        RequestType = request.GetType().FullName,
                        FailureReason = exception.Message,
                        RequestData = new Dictionary<string, object>
                        {
                            [nameof(request.AssemblyName)] = request.AssemblyName,
                            [nameof(request.HandlerRequestFullQualifiedName)] = request.HandlerRequestFullQualifiedName,
                            [nameof(request.Values)] = request.Values
                        }
                    },
                    cancellationToken);

            state.SetHandled(response);
        }
    }
}
