﻿namespace PingPongr
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
        /// <param name="message">The <see cref="IRouteRequest{TResponse}"/> to be handled</param>
        /// <param name="cancellationToken">the cancellation token</param>
        /// <returns></returns>
        Task<TResponse> Handle(TRequest message, CancellationToken cancellationToken);
    }
}
