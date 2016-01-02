namespace PingPongr
{
    using Mediator;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System;

    public class Router : IRouter
    {
        private readonly IDictionary<string, IRoute> routes;

        private readonly IEnumerable<IMediaTypeHandler> mediaHandlers;

        private readonly InstanceFactory factory;

        public Router(IEnumerable<IRoute> routes, IEnumerable<IMediaTypeHandler> mediaHandlers, InstanceFactory factory)
        {
            this.routes = routes.ToDictionary(r => r.Path);
            this.factory = factory;
            this.mediaHandlers = mediaHandlers;
        }

        public async Task RouteRequest(IRequestContext context)
        {
            IRoute route;
            if (routes.TryGetValue(context.Path, out route)) //must match exactly
            {
                var media = mediaHandlers
                    .FirstOrDefault(m => m.CanHandleMediaType(context.RequestMediaType));

                if (media == null)
                    throw new InvalidOperationException("No media handler registered for type " + context.RequestMediaType);

                await route.Send(media, factory, context);

                context.IsHandled = true;
            }
        }
    }
}
