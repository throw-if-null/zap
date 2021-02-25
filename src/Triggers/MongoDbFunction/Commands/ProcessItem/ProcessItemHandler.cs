using MediatR;
using MongoDB.Bson;
using MongoDbFunction.Commands.SendNotification;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDbFunction.Commands.ProcessItem
{
    public class ProcessItemHandler : IRequestHandler<ProcessItemRequest>
    {
        private readonly IMediator _mediator;

        public ProcessItemHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task<Unit> Handle(ProcessItemRequest request, CancellationToken cancellationToken)
        {
            if (!request.Values.TryGetValue("_id", out var id))
                return Unit.Task;

            if (!ObjectId.TryParse(id.ToString(), out var objectId))
                return Unit.Task;

            return _mediator.Send(new SendNotificationRequest { CollectionName = "items", Id = objectId }, cancellationToken);
        }
    }
}
