using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PingPongr.Mediator
{
    /// <summary>
    /// Request/Response mediation patterns
    /// </summary>
    public interface IMediator
    {
        /// <summary>
        /// Publishes the request to multiple handlers
        /// </summary>
        /// <typeparam name="TResponse">Response type</typeparam>
        /// <param name="request">request object</param>
        /// <returns>IEnumerable of the handler responses</returns>
        IEnumerable<TResponse> Publish<TResponse>(IRequest<TResponse> request);

        /// <summary>
        /// Asynchronously publishes the request to multiple handlers
        /// Use: IEnumerable{TResponse} results = await Task.WhenAll(...);
        /// </summary>
        /// <typeparam name="TResponse">Response type</typeparam>
        /// <param name="request">request object</param>
        /// <returns>IEnumerable of tasks representing the publish operations.</returns>
        IEnumerable<Task<TResponse>> PublishAsync<TResponse>(IRequest<TResponse> request);

        /// <summary>
        /// Asynchronously publishes the request to multiple handlers allowing for cancellation
        /// Use: IEnumerable{TResponse} results = await Task.WhenAll(...);
        /// </summary>
        /// <typeparam name="TResponse">Response type</typeparam>
        /// <param name="request">request object</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>IEnumerable of tasks representing the publish operations.</returns>
        IEnumerable<Task<TResponse>> PublishAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken);

        /// <summary>
        /// Sends a request to a single handler
        /// </summary>
        /// <typeparam name="TResponse">the response type</typeparam>
        /// <param name="request">request object</param>
        /// <returns>The handler's response</returns>
        TResponse Send<TResponse>(IRequest<TResponse> request);

        /// <summary>
        /// Asynchronously sends a request to a single handler
        /// </summary>
        /// <typeparam name="TResponse">The response type</typeparam>
        /// <param name="request">the request object</param>
        /// <returns>A task representing the send operation</returns>
        Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request);

        /// <summary>
        /// Asynchronously sends a request to a single handler allowing for cancellation
        /// </summary>
        /// <typeparam name="TResponse">The response type</typeparam>
        /// <param name="request">the request object</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the send operation</returns>
        Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken);
    }
}