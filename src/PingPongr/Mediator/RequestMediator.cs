namespace PingPongr.Mediator
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Threading;

    /// <summary>
    /// Request wrapper for running requests via reflection
    /// This base type is threadsafe (stateless) so it can be cached
    /// Can also be used directly: new RequestMediator{Ping,Pong}().Send(factory, ping);
    /// </summary>
    /// <typeparam name="TRequest">IRequest{TResponse} type</typeparam>
    /// <typeparam name="TResponse">The response type</typeparam>
    public class RequestMediator<TRequest, TResponse> : IRequestMediator<TResponse>
       where TRequest : IRequest<TResponse>
    {
        public TResponse Send(InstanceFactory factory, IRequest<TResponse> msg)
        {
            return factory
                .Create<IRequestHandler<TRequest, TResponse>>()
                .Handle((TRequest)msg);
        }

        public Task<TResponse> SendAsync(InstanceFactory factory, IRequest<TResponse> msg)
        {
            return factory
                .Create<IRequestAsyncHandler<TRequest, TResponse>>()
                .Handle((TRequest)msg);
        }

        public Task<TResponse> SendAsync(InstanceFactory factory, IRequest<TResponse> msg, CancellationToken cancellationToken)
        {
            return factory
                .Create<IRequestCancellableAsyncHandler<TRequest, TResponse>>()
                .Handle((TRequest)msg, cancellationToken);
        }

        public IEnumerable<TResponse> Publish(MultiInstanceFactory factory, IRequest<TResponse> msg)
        {
            return factory
                .Create<IRequestHandler<TRequest, TResponse>>()
                .Select(h => h.Handle((TRequest)msg));
        }

        public IEnumerable<Task<TResponse>> PublishAsync(MultiInstanceFactory factory, IRequest<TResponse> msg)
        {
            return factory
                .Create<IRequestAsyncHandler<TRequest, TResponse>>()
                .Select(h => h.Handle((TRequest)msg));
        }

        public IEnumerable<Task<TResponse>> PublishAsync(MultiInstanceFactory factory, IRequest<TResponse> msg, CancellationToken cancellationToken)
        {
            return factory
                .Create<IRequestCancellableAsyncHandler<TRequest, TResponse>>()
                .Select(h => h.Handle((TRequest)msg, cancellationToken));
        }
    }


}
