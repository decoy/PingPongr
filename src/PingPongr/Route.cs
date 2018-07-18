namespace PingPongr
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// The route wrapper implementation.
    /// A cached (stateless) wrapper for running requests of a specific type
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class Route<TRequest, TResponse> : IRoute
       where TRequest : IRouteRequest<TResponse>
    {
        /// <summary>
        /// The 'default' middleware behavior.
        /// </summary>
        private static readonly RouteMiddlewareDelegate<TResponse> baseMiddleware = () => Task.FromResult(default(TResponse));

        /// <summary>
        /// The path to be matche
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path"></param>
        public Route(string path)
        {
            Path = path;
        }

        /// <summary>
        /// Creates a delegate to be used to run this route's handler from the context services.
        /// </summary>
        /// <param name="context">The context to build the delegate from</param>
        /// <returns></returns>
        public RequestHandlerDelegate<TRequest, TResponse> GetDelegateFromContext(IRequestContext context)
        {
            return (request, cancellationToken) =>
            {
                var behaviors = context.GetService<IEnumerable<IRouteRequestBehavior<TRequest, TResponse>>>();
                var handler = context.GetService<IRouteRequestHandler<TRequest, TResponse>>();

                RequestHandlerDelegate<TResponse> hdelegate = () => handler.Handle(request, cancellationToken);

                var behavior = behaviors
                    .Reverse()
                    .Aggregate(hdelegate, (next, pipeline) => () =>
                    {
                        return pipeline.Handle(request, hdelegate, cancellationToken);
                    });

                return behavior();
            };
        }

        /// <summary>
        /// Runs the request through the specified middlewares
        /// </summary>
        /// <param name="context"></param>
        /// <param name="middlewares"></param>
        /// <returns></returns>
        public Task Send(IRequestContext context, IEnumerable<IRouterMiddleware> middlewares)
        {
            var next = baseMiddleware;

            var handler = GetDelegateFromContext(context);

            foreach (var m in middlewares)
            {
                next = () =>
                {
                    return m.Route(context, handler, next);
                };
            }

            // doesn't return a response
            // the middlewares are responsible for doing something useful with them.
            return next();
        }
    }
}
