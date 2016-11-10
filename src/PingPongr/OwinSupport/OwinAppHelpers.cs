namespace PingPongr.OwinSupport
{
    using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>,
      System.Threading.Tasks.Task>;

    using MidFunc = System.Func<System.Func<System.Collections.Generic.IDictionary<string, object>,
            System.Threading.Tasks.Task>, System.Func<System.Collections.Generic.IDictionary<string, object>,
            System.Threading.Tasks.Task>>;

    using PipeFunc = System.Action<System.Func<System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>, System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>>>;

    public static class OwinAppHelpers
    {
        /// <summary>
        /// Attaches PingPongr to an Owin Middleware pipeline.
        /// </summary>
        /// <param name="pipeline">The pipeline function</param>
        /// <param name="router">The PingPongr router</param>
        /// <param name="routePrefix">An optional prefix filter.  Incoming urls must start with this prefix.</param>
        public static void UsePingPongr(this PipeFunc pipeline, IRouter router, string routePrefix = null)
        {
            pipeline(UsePingPongr(router, routePrefix));
        }

        /// <summary>
        /// Generates a middleware function for PingPongr
        /// </summary>
        /// <param name="router">The PingPongr router</param>
        /// <param name="routePrefix">An optional prefix filter.  Incoming urls must start with this prefix.</param>
        /// <returns>A standard owin middleware function</returns>
        public static MidFunc UsePingPongr(IRouter router, string routePrefix = null)
        {
            return next => async env =>
            {
                var path = (string)env[OwinKeys.RequestPath];

                if (routePrefix == null || (path != null && path.StartsWith(routePrefix)))
                {
                    var context = new OwinContext(env, routePrefix);
                    await router.RouteRequest(context);
                    context.StatusCode = context.IsHandled ? 200 : 404;
                }
                else
                {
                    await next(env);
                }
            };
        }
    }
}
