using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PingPongr.Mediator
{
    public interface IMediator
    {
        IEnumerable<TResponse> Publish<TResponse>(IRequest<TResponse> request);
        IEnumerable<Task<TResponse>> PublishAsync<TResponse>(IRequest<TResponse> request);
        IEnumerable<Task<TResponse>> PublishAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken);
        TResponse Send<TResponse>(IRequest<TResponse> request);
        Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request);
        Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken);
    }
}