using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDbFunction.Commands.SendNotification
{
    public class SendNotificationHandler : IRequestHandler<SendNotificationRequest>
    {
        public Task<Unit> Handle(SendNotificationRequest request, CancellationToken cancellationToken)
        {
            return Unit.Task;
        }
    }
}
