using MediatR;
using Microsoft.Azure.WebJobs;
using MongoDB.Driver;
using MongoDbFunction.Commands.ProcessDbEvent;
using MongoDbTrigger.Triggers;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MongoDbFunction
{
    // https://github.com/Azure/azure-functions-core-tools/issues/2294 - blocked upgrade to .net 5
    public class Function
    {
        private readonly IMediator _mediator;

        public Function(IMediator mediator)
        {
            _mediator = mediator;
        }

        [FunctionName("MongoDbFunction")]
        public Task Run([MongoDbTrigger] ChangeStreamDocument<dynamic> document)
        {
            return _mediator.Send(new ProcessDbEventRequest { Document = document });
        }
    }
}
