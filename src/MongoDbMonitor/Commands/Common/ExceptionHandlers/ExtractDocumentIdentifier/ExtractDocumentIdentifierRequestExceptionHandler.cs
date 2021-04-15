using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDbMonitor.Commands.Common.Responses;
using MongoDbMonitor.Commands.ExtractDocumentIdentifier;
using MongoDbMonitor.Commands.SendSlackAlert;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDbMonitor.Commands.Common.ExceptionHandlers.ExtractDocumentIdentifier
{
    internal abstract class ExtractDocumentIdentifierRequestExceptionHandler<TRequest, TException> :
        IRequestExceptionHandler<TRequest, ProcessingStatusResponse, TException>
        where TRequest : ExtractDocumentIdentifierRequest
        where TException : Exception
    {
        private readonly IMediator _mediator;
        private readonly ExceptionHandlerOptions _options;
        private readonly ILogger _logger;

        protected ExtractDocumentIdentifierRequestExceptionHandler(
            IMediator mediator,
            IOptions<ExceptionHandlerOptions> options,
            ILogger<ExtractDocumentIdentifierRequestExceptionHandler<TRequest, TException>> logger)
        {
            _mediator = mediator;
            _options = options.Value;
            _logger = logger;
        }

        public async Task Handle(
            TRequest request,
            TException exception,
            RequestExceptionHandlerState<ProcessingStatusResponse> state,
            CancellationToken cancellationToken)
        {
            _logger.LogError(exception, exception.Message);

            if (_options.Disabled)
                throw exception;

            var response =
                await
                    _mediator.Send(
                        new SendSlackAlertRequest
                        {
                            RequestType = request.GetType().FullName,
                            FailureReason = exception.Message,
                            RequestData = new Dictionary<string, object>
                            {
                                [nameof(request.CollectionName)] = request.CollectionName,
                                [nameof(request.PropertyToExtract)] = request.PropertyToExtract,
                                [nameof(request.Values)] = request.Values
                            }
                        },
                        cancellationToken);

            response.FinalStep = ProcessingStep.ExtractDocumentIdentifier;

            state.SetHandled(response);
        }
}
}
