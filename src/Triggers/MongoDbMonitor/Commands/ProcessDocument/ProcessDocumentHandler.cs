using MediatR;
using MongoDB.Bson;
using MongoDbMonitor.Commands.Common;
using MongoDbMonitor.Commands.SendNotification;
using System.Collections.Generic;
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

        protected abstract Task<ObjectId> GetObjectId(IDictionary<string, object> values);

        async Task<Unit> IRequestHandler<T, Unit>.Handle(T request, CancellationToken cancellationToken)
        {
            var id = await GetObjectId(request.Values);

            if (id == ObjectId.Empty)
                return Unit.Value;

            await Mediator.Send(new SendNotificationRequest
            {
                CollectionName = request.CollectionName,
                Id = id
            });

            return Unit.Value;
        }
    }
}
