using MediatR;
using MongoDB.Bson;
using MongoDbMonitor.Commands.Exceptions;
using MongoDbMonitor.Commands.SendNotification;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDbMonitor.Commands.ExtractDocumentIdentifier
{
    public abstract class ExtractDocumentIdentifierHandler<T> : IRequestHandler<T, Unit>
        where T : ExtractDocumentIdentifierRequest
    {
        protected IMediator Mediator { get; }

        protected ExtractDocumentIdentifierHandler(IMediator mediator)
        {
            Mediator = mediator;
        }

        async Task<Unit> IRequestHandler<T, Unit>.Handle(T request, CancellationToken cancellationToken)
        {
            if (!request.Values.TryGetValue(request.PropertyToExtract, out var value))
                throw new PropertyNotFoundInDocumentException(request.PropertyToExtract);

            if (!ObjectId.TryParse(value.ToString(), out var id))
                throw new InvalidObjectIdException(value.ToString());

            await Mediator.Send(new SendNotificationRequest
            {
                CollectionName = request.CollectionName,
                Id = id
            });

            return Unit.Value;
        }
    }
}
