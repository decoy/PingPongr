namespace PingPongr
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using PingPongr.Extensions.AspNetCore;
    using System.Collections.Generic;

    /// <summary>
    /// Extensions for the <see cref="IApplicationBuilder"/>
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Attaches the PingPongr <see cref="Router"/> to the ASP.NET middleware pipeline.
        /// This should be the last 'use' in a pipeline.
        /// </summary>
        /// <param name="app">The application builder</param>
        /// <param name="router">The PingPongr <see cref="Router"/></param>
        /// <returns></returns>
        public static IApplicationBuilder UsePingPongr(this IApplicationBuilder app, IRouter router)
        {
            app.Run(async (ctx) =>
            {
                ctx.Response.StatusCode = 200;
                if (!await router.RouteRequest(new AspNetRequestContext(ctx)))
                {
                    ctx.Response.StatusCode = 404;
                }
            });

            return app;
        }

        /// <summary>
        /// Attaches the PingPongr <see cref="Router"/> to the ASP.NET middleware pipeline.
        /// This should be the last 'use' in a pipeline.
        /// Uses the ApplicationServices to resolve <see cref="IRouterMiddleware"/> types and will auto load all routes and middlewares with default options.
        /// </summary>
        /// <param name="app">The application builder</param>
        /// <returns></returns>
        public static IApplicationBuilder UsePingPongr(this IApplicationBuilder app)
        {
            var middlewares = app
                .ApplicationServices
                .GetServices<IRouterMiddleware>();

            return UsePingPongr(app, middlewares);
        }

        /// <summary>
        /// Attaches the PingPongr <see cref="Router"/> to the ASP.NET middleware pipeline with the specified middlewares
        /// </summary>
        /// <param name="app"></param>
        /// <param name="middlewares"></param>
        /// <returns></returns>
        public static IApplicationBuilder UsePingPongr(this IApplicationBuilder app, IEnumerable<IRouterMiddleware> middlewares)
        {
            var routes = RouteBuilder
                .WithLoadedAssemblies()
                .GetRoutes();

            var router = new Router(
                routes,
                middlewares
                );

            return UsePingPongr(app, router);
        }
    }
}
