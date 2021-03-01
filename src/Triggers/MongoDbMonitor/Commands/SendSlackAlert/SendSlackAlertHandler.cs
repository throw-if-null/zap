using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDbMonitor.Commands.SendSlackAlert
{
    internal class SendSlackAlertHandler : IRequestHandler<SendSlackAlertRequest, Unit>
    {
        public Task<Unit> Handle(SendSlackAlertRequest request, CancellationToken cancellationToken)
        {
            return Unit.Task;
        }
    }
}
