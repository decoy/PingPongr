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

        private readonly InstanceFactory factory;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="routes">The list of unique routes to be handled</param>
        /// <param name="mediaHandlers">Available media handlers</param>
        /// <param name="factory">Factory used to generate new request handlers</param>
        public Router(IEnumerable<IRoute> routes, IEnumerable<IMediaTypeHandler> mediaHandlers, InstanceFactory factory)
        {
            this.routes = routes.ToDictionary(r => r.Path);
            this.factory = factory;
            this.mediaHandlers = mediaHandlers;
        }

        public async Task RouteRequest(IRequestContext context)
        {
            context.IsHandled = false;

            IRoute route;
            if (routes.TryGetValue(context.Path, out route)) //must match exactly
            {
                var media = mediaHandlers
                    .FirstOrDefault(m => m.CanHandleMediaType(context.RequestMediaType));

                if (media == null)
                    throw new InvalidOperationException("No media handler registered for type " + context.RequestMediaType);

                context.IsHandled = true;

                await route.Send(media, factory, context);
            }
        }
    }
}
