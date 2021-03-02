using MediatR;
using MongoDB.Driver;
using MongoDbMonitor.Commands.ProcessChangeEvent;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDbMonitor
{
    public sealed class DbMonitor
    {
        private readonly IMediator _mediator;

        public DbMonitor(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task Start(ChangeStreamDocument<dynamic> document, CancellationToken cancellation)
        {
            return _mediator.Send(new ProcessChangeEventRequest
            {
                CollectionName = document.CollectionNamespace.CollectionName,
                OperationType = document.OperationType,
                Values = document.FullDocument
            },
            cancellation);
        }
    }
}
