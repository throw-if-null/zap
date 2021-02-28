using MediatR;
using MongoDbMonitor.Commands.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDbMonitor.Commands.SendNotification
{
    internal class SendNotificationHandler : IErrorHandlingRequestHanlder<SendNotificationRequest, Unit>
    {
        public Task<Unit> Handle(SendNotificationRequest request, CancellationToken cancellationToken)
        {
            return Unit.Task;
        }
    }
}
