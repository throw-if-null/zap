using MediatR;
using System.Collections.Generic;

namespace MongoDbFunction.Commands.ProcessThing
{
    public sealed class ProcessThingRequest : IRequest
    {
        public IDictionary<string, object> Values { get; set; }
    }
}
