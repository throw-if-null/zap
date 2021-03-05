using System.Collections.Generic;

namespace MongoDbMonitor.Commands.Common.Responses
{
    public class ProcessingStatusResponse
    {
        public ProcessingStep FinalStep { get; set; }

        public bool IsSuccessfull => FinalStep == ProcessingStep.Notify;

        public IDictionary<string, int> Perf { get; set; } = new Dictionary<string, int>();
    }
}
