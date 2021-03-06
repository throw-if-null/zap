using MediatR;
using MongoDbMonitor.Commands.Common.Responses;

namespace MongoDbMonitorTest.Data
{
    internal class InvalidRequest : IRequest<ProcessingStatusResponse>
    {
        public int Id { get; set; }
    }
}
