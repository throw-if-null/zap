using MediatR;
using System.Collections.Generic;

namespace MongoDbFunction.Commands.ProcessItem
{
    public sealed class ProcessItemRequest : IRequest
    {
        public IDictionary<string, object> Values { get; set; }
    }
}
