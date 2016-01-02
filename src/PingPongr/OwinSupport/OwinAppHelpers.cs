namespace PingPongr.OwinSupport
{
    using Owin;

    using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>,
      System.Threading.Tasks.Task>;

    using MidFunc = System.Func<System.Func<System.Collections.Generic.IDictionary<string, object>,
            System.Threading.Tasks.Task>, System.Func<System.Collections.Generic.IDictionary<string, object>,
            System.Threading.Tasks.Task>>;

    public static class OwinAppHelpers
    {
        public static IAppBuilder UsePingPongr(this IAppBuilder app, IRouter router, string routePrefix = null)
        {
            return app.Use(UsePingPongr(router, routePrefix));
        }

        public static MidFunc UsePingPongr(IRouter router, string routePrefix = null)
        {
            return next => async env =>
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();
                var context = new OwinContext(env, routePrefix);

                if (routePrefix == null || context.RequestPath.StartsWith(routePrefix))
                {
                    await router.RouteRequest(context);

                    context.StatusCode = context.IsHandled ? 200 : 404;
                }
                System.Console.WriteLine(string.Format("Request {0} handled in {1}ms", context.Path, sw.ElapsedMilliseconds));
                await next(env);
            };
        }
    }
}
