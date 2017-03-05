namespace PingPongr.Sandbox
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.FileProviders;
    using SimpleInjector;
    using System;
    using System.IO;

    /// <summary>
    /// Some middlewares for the sample application
    /// </summary>
    public static class Middleware
    {
        /// <summary>
        /// Setup kestrel for static file hosting for our little demo file
        /// </summary>
        /// <param name="app"></param>
        public static void RegisterFileHosting(this IApplicationBuilder app)
        {
            app.UseDefaultFiles();
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "web")),
                RequestPath = new PathString("/web"),
            });
        }

        /// <summary>
        /// Registers a simple error handling middleware that outputs to the console
        /// </summary>
        /// <param name="app"></param>
        public static void RegisterErrorHandler(this IApplicationBuilder app)
        {
            //Use whatever error handling middleware you want!
            app.Use(next => async (ctx) =>
            {
                try
                {
                    await next(ctx);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("There was an error!: " + ex.Message);
                    ctx.Response.StatusCode = 500;
                }
            });
        }

        /// <summary>
        /// This configures the IHttpContextAccessor accessor per request.
        /// Normally MVC does this as part of its pipeline.
        /// This step isn't required by PingPongr but is used by the example logging handler
        /// </summary>
        /// <param name="app"></param>
        /// <param name="container"></param>
        public static void RegisterContextAccessor(this IApplicationBuilder app, Container container)
        {
            IHttpContextAccessor accessor = container.GetInstance<IHttpContextAccessor>();
            app.Use(next => ctx =>
            {
                accessor.HttpContext = ctx; //set the context appropriately
                return next(ctx);
            });
        }
    }
}
