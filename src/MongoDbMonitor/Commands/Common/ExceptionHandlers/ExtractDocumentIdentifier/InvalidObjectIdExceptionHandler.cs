using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDbMonitor.Commands.Exceptions;
using MongoDbMonitor.Commands.ExtractDocumentIdentifier;

namespace MongoDbMonitor.Commands.Common.ExceptionHandlers.ExtractDocumentIdentifier
{
    internal class InvalidObjectIdExceptionHandler<TRequest> :
        ExtractDocumentIdentifierRequestExceptionHandler<TRequest, InvalidObjectIdException>
        where TRequest : ExtractDocumentIdentifierRequest
    {
        public InvalidObjectIdExceptionHandler(
            IMediator mediator,
            IOptions<ExceptionHandlerOptions> options,
            ILogger<InvalidObjectIdExceptionHandler<TRequest>> logger) :
            base(mediator, options, logger)
        {
        }
    }
}
