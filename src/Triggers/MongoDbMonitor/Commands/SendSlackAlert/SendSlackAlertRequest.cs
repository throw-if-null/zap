using MediatR;
using System.Collections.Generic;

namespace MongoDbMonitor.Commands.SendSlackAlert
{
    public class SendSlackAlertRequest : IRequest
    {
        public string RequestType { get; set; }

        public string FailureReason { get; set; }

        public IDictionary<string, object> RequestData { get; set; }
    }
}
