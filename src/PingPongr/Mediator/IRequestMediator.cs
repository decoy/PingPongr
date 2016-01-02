namespace PingPongr.Mediator
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Wrapper interface used by the mediator and routers
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    public interface IRequestMediator<TResponse>
    {
        TResponse Send(InstanceFactory factory, IRequest<TResponse> msg);
        Task<TResponse> SendAsync(InstanceFactory factory, IRequest<TResponse> msg);
        Task<TResponse> SendAsync(InstanceFactory factory, IRequest<TResponse> msg, CancellationToken cancellationToken);
        IEnumerable<TResponse> Publish(MultiInstanceFactory factory, IRequest<TResponse> msg);
        IEnumerable<Task<TResponse>> PublishAsync(MultiInstanceFactory factory, IRequest<TResponse> msg);
        IEnumerable<Task<TResponse>> PublishAsync(MultiInstanceFactory factory, IRequest<TResponse> msg, CancellationToken cancellationToken);
    }
}
