using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDbMonitor.Commands.Exceptions;

namespace MongoDbMonitor.Commands.Common.ExceptionHandlers.ResolveCollectionType
{
    internal class InvalidRequestTypeExceptionHandler :
        ResolveCollectionTypeRequestExceptionHandler<InvalidRequestTypeException>
    {
        public InvalidRequestTypeExceptionHandler(
            IMediator mediator,
            IOptions<ExceptionHandlerOptions> options,
            ILogger<InvalidRequestTypeExceptionHandler> logger) :
            base(mediator, options, logger)
        {
        }
    }
}
