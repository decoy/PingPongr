namespace PingPongr
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// A delegate describing the Route handle action.
    /// Used to process route middlewares
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="request">The request to be handled</param>
    /// <param name="cancellationToken">the cancellation token</param>
    /// <returns>A response</returns>
    public delegate Task<TResponse> RequestHandlerDelegate<in TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default(CancellationToken));

    /// <summary>
    /// Defines an asynchronous handler for a request
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public interface IRouteRequestHandler<in TRequest, TResponse>
         where TRequest : IRouteRequest<TResponse>
    {
        /// <summary>
        /// Asynchronously handles a request allowing for cancellation
        /// </summary>
        /// <param name="request">The request to be handled</param>
        /// <param name="cancellationToken">the cancellation token</param>
        /// <returns>A response</returns>
        Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken = default(CancellationToken));
    }
}
