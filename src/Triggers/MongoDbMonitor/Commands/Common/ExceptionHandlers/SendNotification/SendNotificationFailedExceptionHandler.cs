using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using MongoDbMonitor.Commands.Common.Responses;
using MongoDbMonitor.Commands.Exceptions;
using MongoDbMonitor.Commands.SendNotification;
using MongoDbMonitor.Commands.SendSlackAlert;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDbMonitor.Commands.Common.ExceptionHandlers.SendNotification
{
    internal class SendNotificationFailedExceptionHandler :
        IRequestExceptionHandler<SendNotificationRequest, ProcessingStatusResponse, SendNotificationFailedException>
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        public SendNotificationFailedExceptionHandler(
            IMediator mediator,
            ILogger<SendNotificationFailedExceptionHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Handle(
            SendNotificationRequest request,
            SendNotificationFailedException exception,
            RequestExceptionHandlerState<ProcessingStatusResponse> state,
            CancellationToken cancellationToken)
        {
            _logger.LogError(
                exception,
                $"Sending notification failed for collection: {request.CollectionName} and Id: {request.Id}");

            var response =
                await
                    _mediator.Send(new SendSlackAlertRequest
                    {
                        RequestType = request.GetType().FullName,
                        FailureReason = exception.Message,
                        RequestData = new Dictionary<string, object>
                        {
                            [nameof(request.CollectionName)] = request.CollectionName,
                            [nameof(request.Id)] = request.Id
                        }
                    },
                    cancellationToken);

            state.SetHandled(response);
        }
    }
}
