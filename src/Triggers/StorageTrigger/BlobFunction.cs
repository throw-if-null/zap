using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace StorageTrigger
{
    public static class BlobFunction
    {
        [FunctionName("BlobFunction")]
        public static void Run([BlobTrigger("test-items/{name}.json", Connection = "AzureWebJobsStorage")]Stream myBlob, string name, ILogger log)
        {
        }
    }
}
