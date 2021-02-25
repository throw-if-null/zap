using MediatR;
using MongoDB.Driver;

namespace MongoDbFunction.Commands.ProcessDbEvent
{
    public sealed class ProcessDbEventRequest : IRequest
    {
        public ChangeStreamDocument<dynamic> Document { get; set; }
    }
}
