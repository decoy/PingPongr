namespace PingPongr
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines an asynchronous handler for a request
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public interface IRouteHandler<in TRequest, TResponse>
    {
        /// <summary>
        /// Asynchronously handles a request allowing for cancellation
        /// </summary>
        /// <param name="request">The <see cref="{TRequest}"/> to be handled</param>
        /// <param name="cancellationToken">the cancellation token</param>
        /// <returns>A <see cref="{TResponse}"/></returns>
        Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
    }
}
