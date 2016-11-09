namespace PingPongr.Sandbox
{
    using Handlers;
    using OwinSupport;
    using System;
    using System.IO;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Hosting;
    using System.Reflection;
    using PingPongr.Serialization.JsonNet;
    using Microsoft.Extensions.Configuration;
    using Microsoft.AspNetCore.Owin;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Text;
    using System.Globalization;
    using Microsoft.AspNetCore.StaticFiles.Infrastructure;
    using Microsoft.Extensions.FileProviders;
    using SimpleInjector.Advanced;
    using SimpleInjector;
    using Services;
    using PingPongr;

    public class Program
    {
        public class Startup
        {
            public void Configure(IApplicationBuilder app)
            {
                var container = BuildContainer();

                //allow us to inject the httpcontext in our handlers
                RegisterContextAccessor(container, app);

                //register PingPongr!
                //Note: PingPongr uses Owin registration for maximum compatibility
                //Include Microsoft.AspNetCore.Owin to use this with Kestrel
                app.UseOwin(pipeline =>
                {
                    pipeline.UsePingPongr(container.GetInstance<Router>(), "/api");
                });

                //setup kestrel for static file hosting for our little demo file
                app.UseDefaultFiles();
                app.UseStaticFiles(new StaticFileOptions()
                {
                    FileProvider = new PhysicalFileProvider(
                        Path.Combine(Directory.GetCurrentDirectory(), "web")),
                    RequestPath = new PathString("/web"),
                });
            }

            public void RegisterContextAccessor(Container container, IApplicationBuilder app)
            {
                //this allows some special magic for thread local access to the http context.
                //normally MVC does this as part of its pipeline.
                IHttpContextAccessor accessor = container.GetInstance<IHttpContextAccessor>();
                app.Use(next => ctx =>
                {
                    accessor.HttpContext = ctx; //set the context appropriately
                    return next(ctx);
                });
            }

            public Container BuildContainer()
            {
                //setup the container
                var container = new Container();
                var assemblies = new[] { typeof(Program).GetTypeInfo().Assembly };

                //this is the router's path to grabbing new request handler instances
                container.RegisterSingleton(new InstanceFactory(container.GetInstance));

                //Our default media handler
                container.RegisterCollection<IMediaTypeHandler>(new[] { new JsonNetMedialHandler() });

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

                //the default router implementation
                container.Register<IRouter, Router>();

                /// https://simpleinjector.org/blog/2016/07/working-around-the-asp-net-core-di-abstraction/
                container.RegisterSingleton<IHttpContextAccessor, HttpContextAccessor>();
                container.RegisterSingleton<IUserContext, AspNetUserContext>();

                container.Verify();

                return container;
            }
        }

        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
               .UseKestrel()
               .UseStartup<Startup>()
               .UseUrls("http://localhost:12345")
               .Build();

            host.Run();
        }
    }
}
