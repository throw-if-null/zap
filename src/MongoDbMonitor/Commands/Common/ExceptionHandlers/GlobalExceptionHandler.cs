using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDbMonitor.Commands.Common.ExceptionHandlers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDbMonitor.Commands.Common
{
    public class GlobalExceptionHandler<TRequest, TResponse, TException> :
        IRequestExceptionHandler<TRequest, TResponse, TException>
        where TException : Exception
        where TResponse : new()
    {
        private readonly ExceptionHandlerOptions _options;
        private readonly ILogger _logger;

        public GlobalExceptionHandler(
            IOptions<ExceptionHandlerOptions> options,
            ILogger<GlobalExceptionHandler<TRequest, TResponse, TException>> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public Task Handle(
            TRequest request,
            TException exception,
            RequestExceptionHandlerState<TResponse> state,
            CancellationToken cancellationToken)
        {
            _logger.LogCritical(exception, exception.Message);

            if (_options.Disabled && !_options.OnlyGlobal)
                return Task.CompletedTask;

            state.SetHandled(new TResponse());

            return Task.CompletedTask;
        }
    }
}
