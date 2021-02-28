using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDbMonitor.Commands.SendNotification
{
    internal class SendNotificationHandler : IRequestHandler<SendNotificationRequest>
    {
        public Task<Unit> Handle(SendNotificationRequest request, CancellationToken cancellationToken)
        {
            return Unit.Task;
        }
    }
}
