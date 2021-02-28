using MediatR;

namespace MongoDbMonitor.Commands.Common
{
    public interface IErrorHandlingRequestHanlder<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>, IOnRequestProcessingError
    {
    }
}
