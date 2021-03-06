using MediatR;
using Microsoft.Extensions.Logging;
using MongoDbMonitor.Commands.Exceptions;
using MongoDbMonitor.Commands.ExtractDocumentIdentifier;

namespace MongoDbMonitor.Commands.Common.ExceptionHandlers.ExtractDocumentIdentifier
{
    internal class PropertyNotFoundInDocumentExceptionHandler<TRequest> :
        ExtractDocumentIdentifierRequestExceptionHandler<TRequest, PropertyNotFoundInDocumentException>
        where TRequest : ExtractDocumentIdentifierRequest
    {
        public PropertyNotFoundInDocumentExceptionHandler(
            IMediator mediator,
            ILogger<PropertyNotFoundInDocumentExceptionHandler<TRequest>> logger) :
            base(mediator, logger)
        {
        }
    }
}
