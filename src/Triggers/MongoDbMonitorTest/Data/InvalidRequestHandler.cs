using MediatR;
using MongoDbMonitor.Commands.Common.Responses;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDbMonitorTest.Data
{
    internal class InvalidRequestHandler : IRequestHandler<InvalidRequest, ProcessingStatusResponse>
    {
        public Task<ProcessingStatusResponse> Handle(InvalidRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new ProcessingStatusResponse());
        }
    }
}
