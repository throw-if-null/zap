using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDbMonitor.Commands.Exceptions;

namespace MongoDbMonitor.Commands.Common.ExceptionHandlers.ResolveCollectionType
{
    internal class MissingRequiredPropertyExceptionHandler :
        ResolveCollectionTypeRequestExceptionHandler<MissingRequiredPropertyException>
    {
        public MissingRequiredPropertyExceptionHandler(
            IMediator mediator,
            IOptions<ExceptionHandlerOptions> options,
            ILogger<MissingRequiredPropertyExceptionHandler> logger) :
            base(mediator, options, logger)
        {
        }
    }
}