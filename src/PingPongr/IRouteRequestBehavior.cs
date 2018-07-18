namespace PingPongr
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// A delegate for running the next item in the handler chain.  (may be another behavior or the actual handler)
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    /// <returns></returns>
    public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();

    /// <summary>
    /// A route handler behavior.  Allows wrapping a request similar to decorators but without requiring DI container support.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public interface IRouteRequestBehavior<in TRequest, TResponse>
        where TRequest : IRouteRequest<TResponse>
    {
        /// <summary>
        /// The Handler behavior.  Await the <see cref="RequestHandlerDelegate{TResponse}"/> to run the actual action.
        /// </summary>
        /// <param name="request">The request to be handled</param>
        /// <param name="next">Awaitable delegate for the next action in the pipeline</param>
        /// <param name="cancellationToken">the cancellation token</param>
        /// <returns></returns>
        Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken = default(CancellationToken));
    }
}
