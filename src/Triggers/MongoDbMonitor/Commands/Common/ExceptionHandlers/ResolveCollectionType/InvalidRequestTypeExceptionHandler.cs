using MediatR;
using Microsoft.Extensions.Logging;
using MongoDbMonitor.Commands.Exceptions;

namespace MongoDbMonitor.Commands.Common.ExceptionHandlers.ResolveCollectionType
{
    internal class InvalidRequestTypeExceptionHandler :
        ResolveCollectionTypeRequestExceptionHandler<InvalidRequestTypeException>
    {
        public InvalidRequestTypeExceptionHandler(
            IMediator mediator,
            ILogger<InvalidRequestTypeExceptionHandler> logger) :
            base(mediator, logger)
        {
        }
    }
}
