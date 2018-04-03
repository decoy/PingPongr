namespace PingPongr
{
    using System.Threading;
    using System.Threading.Tasks;

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
        Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
    }
}
