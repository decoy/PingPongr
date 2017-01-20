﻿namespace PingPongr.Sandbox
{
    using Handlers;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.FileProviders;
    using PingPongr.OwinSupport;
    using PingPongr.Serialization.JsonNet;
    using Services;
    using SimpleInjector;
    using System;
    using System.IO;
    using System.Reflection;

    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            var container = BuildContainer();

            //register our optional example app middlewares.
            RegisterErrorHandler(app); //must be registered first
            RegisterContextAccessor(container, app);
            RegisterFileHosting(app);

            //register PingPongr!
            //Note: PingPongr uses Owin registration for maximum compatibility
            //Include package Microsoft.AspNetCore.Owin to use this with Kestrel
            app.UseOwin(owin =>
            {
                owin.UsePingPongr(container.GetInstance<Router>(), "/api");
            });
        }

        public static void RegisterFileHosting(IApplicationBuilder app)
        {
            //setup kestrel for static file hosting for our little demo file
            app.UseDefaultFiles();
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "web")),
                RequestPath = new PathString("/web"),
            });
        }

        public static void RegisterErrorHandler(IApplicationBuilder app)
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

        public static void RegisterContextAccessor(Container container, IApplicationBuilder app)
        {
            //this allows some special magic for thread local access to the http context.
            //normally MVC does this as part of its pipeline.
            //this isn't required by PingPongr but is used by the example logging handler
            IHttpContextAccessor accessor = container.GetInstance<IHttpContextAccessor>();
            app.Use(next => ctx =>
            {
                accessor.HttpContext = ctx; //set the context appropriately
                return next(ctx);
            });
        }

        public static Container BuildContainer()
        {
            //setup the container
            var container = new Container();
            var assemblies = new[] { typeof(Program).GetTypeInfo().Assembly };

            //TODO more examples!
            container.Register(typeof(IRouteRequestHandler<,>), assemblies);
            container.RegisterDecorator(typeof(IRouteRequestHandler<,>), typeof(LoggingDecorator<,>));

            //build our routes and register.
            var routes = RouteBuilder
                .WithAssemblies(assemblies)  //limit our search to this assembly
                .Filter(t => t.FullName.StartsWith("PingPongr.Sandbox.Api")) //limit to api namespace
                .Path(t => "/" + t.Name.ToLower()) //override default path naming
                .GetRoutes();
            container.RegisterCollection(routes);

            //register the default media handler
            //Note, there can be more than one, so this is a collection
            container.RegisterCollection<IMediaTypeHandler>(new[]
            {
                typeof(JsonNetMediaHandler),
            });

            //this is the router's path to grabbing new request handler instances
            container.RegisterSingleton(new InstanceFactory(container.GetInstance));

            //the default router implementation
            container.Register<IRouter, Router>();

            //https://simpleinjector.org/blog/2016/07/working-around-the-asp-net-core-di-abstraction/
            container.RegisterSingleton<IHttpContextAccessor, HttpContextAccessor>();
            container.RegisterSingleton<IUserContext, AspNetUserContext>();

            container.Verify();

            return container;
        }
    }
}
