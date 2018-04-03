namespace PingPongr
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// The default implementation of the router.  
    /// Paths are matched to routes exactly.
    /// First matching media handler is used to process the request
    /// </summary>
    public class Router : IRouter
    {
        private readonly IDictionary<string, IRoute> routes;

        private readonly IEnumerable<IMediaTypeHandler> mediaHandlers;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="routes">The list of unique routes to be handled</param>
        /// <param name="mediaHandlers">Available media handlers</param>
        public Router(IEnumerable<IRoute> routes, IEnumerable<IMediaTypeHandler> mediaHandlers)
        {
            this.routes = routes.ToDictionary(r => r.Path);
            this.mediaHandlers = mediaHandlers;
        }

        /// <summary>
        /// Routes the request represented by the context to the appropriate route.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task RouteRequest(IRequestContext context)
        {
            context.IsHandled = false;

            if (routes.TryGetValue(context.Path, out var route)) //must match exactly
            {
                var media = mediaHandlers
                    .FirstOrDefault(m => m.CanHandle(context.RequestContentType));

                if (media == null)
                    throw new InvalidOperationException("No media handler registered for type " + context.RequestContentType);

                context.IsHandled = true;

                await route.Send(media, context);
            }
        }
    }
}
