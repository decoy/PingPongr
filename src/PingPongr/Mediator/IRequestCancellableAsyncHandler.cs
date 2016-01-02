namespace PingPongr.Mediator
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines an asynchronous handler for a request
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public interface IRequestCancellableAsyncHandler<in TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        /// <summary>
        /// Asynchronously handles a request
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TResponse> Handle(TRequest message, CancellationToken cancellationToken);
    }
}
