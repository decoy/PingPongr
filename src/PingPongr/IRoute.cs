﻿namespace PingPongr
{
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a route action for a particular path.
    /// </summary>
    public interface IRoute
    {
        /// <summary>
        /// the path this route handles
        /// </summary>
        string Path { get; }

        /// <summary>
        /// The action to happen when routing
        /// </summary>
        /// <param name="mediaHandler">The media handler for reading the request</param>
        /// <param name="context">the request context</param>
        /// <returns>An awaitable task representing the send action.</returns>
        Task Send(IMediaTypeHandler mediaHandler, IRequestContext context);
    }
}
