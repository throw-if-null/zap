using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using MongoDbMonitor.Commands.ExtractDocumentIdentifier;
using MongoDbMonitor.Commands.SendSlackAlert;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDbMonitor.Commands.Common.ExceptionHandlers.ExtractDocumentIdentifier
{
    internal abstract class ExtractDocumentIdentifierRequestExceptionHandler<TException> :
        IRequestExceptionHandler<ExtractDocumentIdentifierRequest, Unit, TException>
        where TException : Exception
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        protected ExtractDocumentIdentifierRequestExceptionHandler(
            IMediator mediator,
            ILogger<ExtractDocumentIdentifierRequestExceptionHandler<TException>> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Handle(
            ExtractDocumentIdentifierRequest request,
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
                            [nameof(request.CollectionName)] = request.CollectionName,
                            [nameof(request.PropertyToExtract)] = request.PropertyToExtract,
                            [nameof(request.Values)] = request.Values
                        }
                    },
                    cancellationToken);

            state.SetHandled(response);
        }
    }
}
