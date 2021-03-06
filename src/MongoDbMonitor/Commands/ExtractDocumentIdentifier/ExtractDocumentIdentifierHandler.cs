using MediatR;
using MongoDB.Bson;
using MongoDbMonitor.Commands.Common.Responses;
using MongoDbMonitor.Commands.Exceptions;
using MongoDbMonitor.Commands.SendNotification;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDbMonitor.Commands.ExtractDocumentIdentifier
{
    public abstract class ExtractDocumentIdentifierHandler<T> : IRequestHandler<T, ProcessingStatusResponse>
        where T : ExtractDocumentIdentifierRequest
    {
        protected IMediator Mediator { get; }

        protected ExtractDocumentIdentifierHandler(IMediator mediator)
        {
            Mediator = mediator;
        }

        async Task<ProcessingStatusResponse> IRequestHandler<T, ProcessingStatusResponse>.Handle(T request, CancellationToken cancellationToken)
        {
            if (!request.Values.TryGetValue(request.PropertyToExtract, out var value))
                throw new PropertyNotFoundInDocumentException(request.PropertyToExtract);

            if (!ObjectId.TryParse(value.ToString(), out var id))
                throw new InvalidObjectIdException(value.ToString());

            var response = await Mediator.Send(new SendNotificationRequest
            {
                CollectionName = request.CollectionName,
                Id = id
            });

            return response;
        }
    }
}
