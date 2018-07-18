namespace PingPongr
{
    using System.Threading.Tasks;

    /// <summary>
    /// Describes the 'next' middleware delegate
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    /// <returns>Returns the response from the next middleware</returns>
    public delegate Task<TResponse> RouteMiddlewareDelegate<TResponse>();

    /// <summary>
    /// Defines a middleware for PingPongr.
    /// This doesn't follow the delegate middleware pattern (used by ASP.NET)
    /// because the route function has to be generic.
    /// </summary>
    public interface IRouterMiddleware
    {
        /// <summary>
        /// The action to run when called from a route
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="context">The request context</param>
        /// <param name="handler">The handler delegate</param>
        /// <param name="next">The next middleware delegate</param>
        /// <returns></returns>
        Task<TResponse> Route<TRequest, TResponse>(IRequestContext context, RequestHandlerDelegate<TRequest, TResponse> handler, RouteMiddlewareDelegate<TResponse> next);
    }
}
