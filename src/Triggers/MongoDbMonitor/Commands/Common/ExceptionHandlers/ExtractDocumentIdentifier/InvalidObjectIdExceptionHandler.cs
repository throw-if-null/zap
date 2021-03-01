using MediatR;
using Microsoft.Extensions.Logging;
using MongoDbMonitor.Commands.Exceptions;

namespace MongoDbMonitor.Commands.Common.ExceptionHandlers.ExtractDocumentIdentifier
{
    internal class InvalidObjectIdExceptionHandler :
        ExtractDocumentIdentifierRequestExceptionHandler<InvalidObjectIdException>
    {
        public InvalidObjectIdExceptionHandler(
            IMediator mediator,
            ILogger<InvalidObjectIdExceptionHandler> logger) :
            base(mediator, logger)
        {
        }
    }
}
