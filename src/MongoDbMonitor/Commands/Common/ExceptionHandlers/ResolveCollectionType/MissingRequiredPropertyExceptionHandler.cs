using MediatR;
using Microsoft.Extensions.Logging;
using MongoDbMonitor.Commands.Exceptions;

namespace MongoDbMonitor.Commands.Common.ExceptionHandlers.ResolveCollectionType
{
    internal class MissingRequiredPropertyExceptionHandler :
        ResolveCollectionTypeRequestExceptionHandler<MissingRequiredPropertyException>
    {
        public MissingRequiredPropertyExceptionHandler(
            IMediator mediator,
            ILogger<MissingRequiredPropertyExceptionHandler> logger) :
            base(mediator, logger)
        {
        }
    }
}