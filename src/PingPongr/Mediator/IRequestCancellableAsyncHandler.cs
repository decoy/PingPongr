namespace PingPongr.Mediator
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IRequestCancellableAsyncHandler<in TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        Task<TResponse> Handle(TRequest message, CancellationToken cancellationToken);
    }
}
