using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDbMonitor.Commands.Common.Responses;
using MongoDbMonitor.Commands.ResolveCollectionType;
using MongoDbMonitor.Commands.SendSlackAlert;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDbMonitor.Commands.Common.ExceptionHandlers.ResolveCollectionType
{
    internal abstract class ResolveCollectionTypeRequestExceptionHandler<TException> :
        IRequestExceptionHandler<ResolveCollectionTypeRequest, ProcessingStatusResponse, TException>
        where TException : Exception
    {
        private readonly IMediator _mediator;
        private readonly ExceptionHandlerOptions _options;
        private readonly ILogger _logger;

        protected ResolveCollectionTypeRequestExceptionHandler(
            IMediator mediator,
            IOptions<ExceptionHandlerOptions> options,
            ILogger<ResolveCollectionTypeRequestExceptionHandler<TException>> logger)
        {
            _mediator = mediator;
            _options = options.Value;
            _logger = logger;
        }

        public async Task Handle(
            ResolveCollectionTypeRequest request,
            TException exception,
            RequestExceptionHandlerState<ProcessingStatusResponse> state,
            CancellationToken cancellationToken)
        {
            _logger.LogError(exception, exception.Message);

            if (_options.Disabled)
                throw exception;

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

            response.FinalStep = ProcessingStep.ResolveCollectionType;

            state.SetHandled(response);
        }
    }
}
