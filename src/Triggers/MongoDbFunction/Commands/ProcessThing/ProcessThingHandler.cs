using MediatR;
using MongoDB.Bson;
using MongoDbFunction.Commands.SendNotification;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDbFunction.Commands.ProcessThing
{
    public class ProcessThingHandler : IRequestHandler<ProcessThingRequest>
    {
        private readonly IMediator _mediator;

        public ProcessThingHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task<Unit> Handle(ProcessThingRequest request, CancellationToken cancellationToken)
        {
            if (!request.Values.TryGetValue("_id", out var id))
                return Unit.Task;

            if (!ObjectId.TryParse(id.ToString(), out var objectId))
                return Unit.Task;

            return _mediator.Send(new SendNotificationRequest { CollectionName = "things", Id = objectId }, cancellationToken);
        }
    }
}
