using MediatR;
using MongoDbMonitor.Commands.Exceptions;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDbMonitor.Commands.SendNotification
{
    internal class SendNotificationHandler : IRequestHandler<SendNotificationRequest, Unit>
    {
        public Task<Unit> Handle(SendNotificationRequest request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new SendNotificationFailedException();

            return Unit.Task;
        }
    }
}
