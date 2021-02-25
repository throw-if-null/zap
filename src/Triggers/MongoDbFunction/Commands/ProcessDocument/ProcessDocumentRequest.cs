using MediatR;
using System.Collections.Generic;

namespace MongoDbFunction.Commands.ProcessDocument
{
    public sealed class ProcessDocumentRequest : IRequest
    {
        public string CollectionName { get; set; }

        public IDictionary<string, object> Values { get; set; }
    }
}
