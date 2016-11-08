namespace PingPongr.OwinSupport
{
    //using Owin;

    using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>,
      System.Threading.Tasks.Task>;

    using MidFunc = System.Func<System.Func<System.Collections.Generic.IDictionary<string, object>,
            System.Threading.Tasks.Task>, System.Func<System.Collections.Generic.IDictionary<string, object>,
            System.Threading.Tasks.Task>>;

    public static class OwinAppHelpers
    {
        /// <summary>
        /// Use the PingPongr as Owin middleware.
        /// </summary>
        /// <param name="app">The app to attach to</param>
        /// <param name="router">an instance of a PingPongr router</param>
        /// <param name="routePrefix">Optional route prefix.  The router will only handle paths that have this prefix.  If null, will handle all paths.</param>
        /// <returns></returns>
        //public static IAppBuilder UsePingPongr(this IAppBuilder app, IRouter router, string routePrefix = null)
        //{
        //    return app.Use(UsePingPongr(router, routePrefix));
        //}

        public static MidFunc UsePingPongr(IRouter router, string routePrefix = null)
        {
            return next => async env =>
            {
                var context = new OwinContext(env, routePrefix);

                if (routePrefix == null || context.RequestPath.StartsWith(routePrefix))
                {
                    await router.RouteRequest(context);

                    context.StatusCode = context.IsHandled ? 200 : 404;
                }
                await next(env);
            };
        }
    }
}
