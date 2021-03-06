using MediatR;
using MongoDbMonitor.Commands.Common.Responses;
using System.Collections.Generic;

namespace MongoDbMonitor.Commands.SendSlackAlert
{
    public class SendSlackAlertRequest : IRequest<ProcessingStatusResponse>
    {
        public string RequestType { get; set; }

        public string FailureReason { get; set; }

        public IDictionary<string, object> RequestData { get; set; }
    }
}
