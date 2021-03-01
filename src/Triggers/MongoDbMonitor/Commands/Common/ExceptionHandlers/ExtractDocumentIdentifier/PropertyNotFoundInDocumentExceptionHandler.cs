using MediatR;
using Microsoft.Extensions.Logging;
using MongoDbMonitor.Commands.Exceptions;

namespace MongoDbMonitor.Commands.Common.ExceptionHandlers.ExtractDocumentIdentifier
{
    internal class PropertyNotFoundInDocumentExceptionHandler :
        ExtractDocumentIdentifierRequestExceptionHandler<PropertyNotFoundInDocumentException>
    {
        public PropertyNotFoundInDocumentExceptionHandler(
            IMediator mediator,
            ILogger<PropertyNotFoundInDocumentExceptionHandler> logger) :
            base(mediator, logger)
        {
        }
    }
}
