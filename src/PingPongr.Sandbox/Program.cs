namespace PingPongr.Sandbox
{
    using Owin;
    using OwinSupport;
    using System;
    using System.Linq;
    using SimpleInjector;
    using Mediator;
    using Handlers;

    public class Program
    {
        public class Startup
        {
            public void Configuration(IAppBuilder app)
            {
                var container = BuildContainer();

                app.UseSimpleInjectorMiddleware(container);
                app.UsePingPongr(container.GetInstance<IRouter>(), "/api");
                app.UseWelcomePage("/");
            }

            public Container BuildContainer()
            {
                //setup the container
                var container = new Container();
                var assemblies = new[] { typeof(Program).Assembly };

                //this is the router's path to grabbing new request handler instances
                container.RegisterSingleton(new InstanceFactory(container.GetInstance));

                //Our default media handler
                container.RegisterCollection<IMediaTypeHandler>(new[] { new DefaultJsonMediaHandler() });

                //TODO more examples!
                container.Register(typeof(IRequestAsyncHandler<,>), assemblies);
                container.RegisterDecorator(typeof(IRequestAsyncHandler<,>), typeof(LoggingDecorator<,>));

                //build our routes and register.
                var routes = RouteBuilder
                    .WithAssemblies(assemblies)  //limit our search to this assembly
                    .Filter(t => t.FullName.StartsWith("PingPongr.Sandbox.Api")) //limit to api namespace
                    .Path(t => "/" + t.Name.ToLower()) //override default path naming
                    .GetRoutes();
                container.RegisterCollection(routes);

                //the default router implementation
                container.Register<IRouter, Router>();  

                //get owin context from handlers: http://simpleinjector.readthedocs.org/en/latest/owinintegration.html
                container.RegisterSingleton<IOwinContextProvider>(new CallContextOwinContextProvider());

                container.Verify();

                return container;
            }
        }

        public static void Main(string[] args)
        {
            using (Microsoft.Owin.Hosting.WebApp.Start<Startup>("http://localhost:12345"))
            {
                Console.WriteLine("Started...");
                Console.ReadLine();
            }
        }
    }
}
