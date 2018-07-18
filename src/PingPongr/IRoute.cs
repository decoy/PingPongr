namespace PingPongr
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a route action for a particular path.
    /// </summary>
    public interface IRoute
    {
        /// <summary>
        /// The path this route handles
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Runs the request through the specified middlewares
        /// </summary>
        /// <param name="context">The request context</param>
        /// <param name="middlewares">The middlewares that will process this request</param>
        /// <returns></returns>
        Task Send(IRequestContext context, IEnumerable<IRouterMiddleware> middlewares);
    }
}
