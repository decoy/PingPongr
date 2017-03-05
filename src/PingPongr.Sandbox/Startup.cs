namespace PingPongr.Sandbox
{
    using Handlers;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using PingPongr.Serialization.JsonNet;
    using Services;
    using SimpleInjector;
    using System.Reflection;

    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            var container = BuildContainer();

            //register our optional example app middlewares.
            //When a request comes in, it goes through these in order.
            app.RegisterErrorHandler();
            app.RegisterFileHosting();

            //this creates a simpleinjector scope per-request
            //use in conjunction with Lifestyle.Scoped registrations in the container
            app.UseSimpleInjectorAspNetRequestScoping(container);

            app.RegisterContextAccessor(container);

            //register PingPongr!
            //Note: PingPongr uses Owin registration for maximum compatibility
            //Include package Microsoft.AspNetCore.Owin to use this with Kestrel
            app.UseOwin(owin =>
            {
                owin.UsePingPongr(container.GetInstance<IRouter>(), "/api");
            });
        }

        public static Container BuildContainer()
        {
            var container = new Container();
            var assemblies = new[] { typeof(Program).GetTypeInfo().Assembly };

            //register the routes and decorators with the container
            //the syntax for this is different per container.  see your container's documentation for details
            container.Register(typeof(IRouteRequestHandler<,>), assemblies);
            container.RegisterDecorator(typeof(IRouteRequestHandler<,>), typeof(LoggingDecorator<,>));

            //build our routes and register.
            var routes = RouteBuilder
                .WithAssemblies(assemblies)  //limit our search to this assembly
                .Filter(t => t.FullName.StartsWith("PingPongr.Sandbox.Api")) //limit to api namespace
                .Path(t => "/" + t.Name.ToLower()) //override default path naming
                .GetRoutes();

            //Using the PingPongr.Serialization.JsonNet media handler
            var mediaHandlers = new[] { JsonNetMediaHandler.CreateDefault() };

            //this is the router's path to grabbing new request handler instances
            var factory = new InstanceFactory(container.GetInstance);

            //create the router and register it
            var router = new Router(routes, mediaHandlers, factory);
            container.RegisterSingleton<IRouter>(router);

            //register some simple services to injecto into handlers
            //https://simpleinjector.org/blog/2016/07/working-around-the-asp-net-core-di-abstraction/
            container.RegisterSingleton<IHttpContextAccessor, HttpContextAccessor>();
            container.RegisterSingleton<IUserContext, AspNetUserContext>();

            container.Verify();

            return container;
        }
    }
}
