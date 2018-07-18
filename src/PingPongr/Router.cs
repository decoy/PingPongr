namespace PingPongr
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// The default implementation of the router.  
    /// Paths are matched to routes exactly.
    /// Actual processing is handled by middlewares
    /// </summary>
    public class Router : IRouter
    {
        private readonly IDictionary<string, IRoute> routes;

        private readonly IEnumerable<IRouterMiddleware> middlewares;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="routes">The list of unique routes to be handled</param>
        /// <param name="middlewares">Middlewares that process the routes</param>
        public Router(IEnumerable<IRoute> routes, IEnumerable<IRouterMiddleware> middlewares)
        {
            // TODO - do some more advanced error checking here
            // dupe paths and no middleware warnings/errors
            this.routes = routes.ToDictionary(r => r.Path);
            this.middlewares = middlewares;
        }

        /// <summary>
        /// Routes the request represented by the context to the appropriate route.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Returns whether or not the request was matched a route</returns>
        public async Task<bool> RouteRequest(IRequestContext context)
        {
            if (routes.TryGetValue(context.Path, out var route)) //must match exactly
            {
                await route.Send(context, middlewares);
                return true;
            }

            return false;
        }
    }
}
