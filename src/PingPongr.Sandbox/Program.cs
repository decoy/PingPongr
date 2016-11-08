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

    public class Program
    {
        public class Startup
        {
            public void Configure(IApplicationBuilder app)
            {
                var assemblies = new[] { typeof(Program).GetTypeInfo().Assembly };

                var routes = RouteBuilder
                    .WithAssemblies(assemblies)  //limit our search to this assembly
                    .Filter(t => t.FullName.StartsWith("PingPongr.Sandbox.Api")) //limit to api namespace
                    .Path(t => "/" + t.Name.ToLower()) //override default path naming
                    .GetRoutes();

                var media = new[] { new JsonNetMedialHandler() };
                var router = new Router(routes, media, t => new PingHandler());

                app.UseOwin(pipeline =>
                {
                    pipeline.UsePingPongr(router, "/api");
                });

                app.UseStaticFiles(new StaticFileOptions()
                {
                    FileProvider = new PhysicalFileProvider(
                        Path.Combine(Directory.GetCurrentDirectory(), "web")),
                    RequestPath = new PathString("/web"),
                });
            }
        }


        //{
        //    public class Startup
        //    {
        //        public void Configuration(IAppBuilder app)
        //        {
        //            var container = BuildContainer();

        //            app.UseSimpleInjectorMiddleware(container);
        //            app.Use(OwinAppHelpers.UsePingPongr(container.GetInstance<IRouter>(), "/api"));
        //            //app.UsePingPongr(container.GetInstance<IRouter>(), "/api");

        //            //static file hosting for the test client
        //            var dir = Directory.CreateDirectory("./web");
        //            Console.WriteLine("Hosting files from: " + dir.FullName);
        //            app.UseFileServer(new FileServerOptions()
        //            {
        //                FileSystem = new PhysicalFileSystem(dir.FullName),
        //                RequestPath = new PathString("/web"),
        //            });

        //            app.UseWelcomePage("/");
        //        }

        //        public Container BuildContainer()
        //        {
        //            //setup the container
        //            var container = new Container();
        //            var assemblies = new[] { typeof(Program).Assembly };

        //            //this is the router's path to grabbing new request handler instances
        //            container.RegisterSingleton(new InstanceFactory(container.GetInstance));

        //            //Our default media handler
        //            container.RegisterCollection<IMediaTypeHandler>(new[] { new DefaultJsonMediaHandler() });

        //            //TODO more examples!
        //            container.Register(typeof(IRouteRequestHandler<,>), assemblies);
        //            container.RegisterDecorator(typeof(IRouteRequestHandler<,>), typeof(LoggingDecorator<,>));

        //            //build our routes and register.
        //            var routes = RouteBuilder
        //                .WithAssemblies(assemblies)  //limit our search to this assembly
        //                .Filter(t => t.FullName.StartsWith("PingPongr.Sandbox.Api")) //limit to api namespace
        //                .Path(t => "/" + t.Name.ToLower()) //override default path naming
        //                .GetRoutes();
        //            container.RegisterCollection(routes);

        //            //the default router implementation
        //            container.Register<IRouter, Router>();

        //            //get owin context from handlers: http://simpleinjector.readthedocs.org/en/latest/owinintegration.html
        //            container.RegisterSingleton<IOwinContextProvider>(new CallContextOwinContextProvider());

        //            container.Verify();

        //            return container;
        //        }
        //    }

        public static void Main(string[] args)
        {
            //var dir = Directory.CreateDirectory("./web");
            //Console.WriteLine("Hosting files from: " + dir.FullName);

            var host = new WebHostBuilder()
               .UseKestrel()
               .UseStartup<Startup>()
               .UseUrls("http://localhost:12345")
               .Build();

            host.Run();
        }
    }
}
