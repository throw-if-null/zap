using MediatR;
using MongoDB.Bson;
using MongoDbMonitor.Commands.Common;
using MongoDbMonitor.Commands.Exceptions;
using MongoDbMonitor.Commands.SendNotification;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDbMonitor.Commands.ProcessDocument
{
    public abstract class ProcessDocumentHandler<T> : IErrorHandlingRequestHanlder<T, Unit>
        where T : ProcessDocumentRequest
    {
        protected IMediator Mediator { get; }

        protected ProcessDocumentHandler(IMediator mediator)
        {
            Mediator = mediator;
        }

        protected abstract string PropertyToExtract { get; }

        async Task<Unit> IRequestHandler<T, Unit>.Handle(T request, CancellationToken cancellationToken)
        {
            if (!request.Values.TryGetValue(PropertyToExtract, out var value))
                throw new PropertyNotFoundInDocumentException(PropertyToExtract);

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
