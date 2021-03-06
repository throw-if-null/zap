using MediatR;
using MongoDbMonitor.Clients.HttpApi;
using MongoDbMonitor.Commands.Common.Responses;
using MongoDbMonitor.Commands.Exceptions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDbMonitor.Commands.SendNotification
{
    internal class SendNotificationHandler : IRequestHandler<SendNotificationRequest, ProcessingStatusResponse>
    {
        private readonly IHttpApiClient _client;

        public SendNotificationHandler(IHttpApiClient client)
        {
            _client = client;
        }

        public async Task<ProcessingStatusResponse> Handle(SendNotificationRequest request, CancellationToken cancellationToken)
        {
            try
            {
                await _client.Notify(request.CollectionName, request.Id, cancellationToken);

                return new ProcessingStatusResponse { FinalStep = ProcessingStep.Notify };
            }
            catch (Exception e)
            {
                throw new SendNotificationFailedException(e);
            }
        }
    }
}
