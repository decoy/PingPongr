using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PingPongr;
using PingPongr.Serialization.JsonNet;
using System.IO;

namespace Examples.Complex
{
    using Handlers;
    using Services;

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // add the route handlers to the service registrations.
            services.AddRouteHandlers();

            // Add the context accessor.  This allows us to inject the http context into handlers.
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // our greeting service
            services.AddSingleton<GreetingService>();

            // Support for things like decorators depends on the dependency injection container.
            // In this case, we're using scrutor to help with the open generic decorators
            // https://github.com/khellang/Scrutor
            services.Decorate(typeof(IRouteRequestHandler<,>), typeof(LoggingDecorator<,>));
        }

        public void Configure(IApplicationBuilder app)
        {
            app.Map("/api", api =>
            {
                // overriding the default route path
                var routes = RouteBuilder
                    .WithLoadedAssemblies()
                    .WithPathBuilder(t => "/" + t.Name.ToLower())
                    .GetRoutes();

                // customizing the json settings
                var settings = new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };

                var media = new JsonNetMiddleware(JsonSerializer.Create(settings));

                // our custom exception handling middleware
                var errors = new ExceptionMiddleware();

                // Middleware order is important, just like in ASP.NET
                // define them 'outer' to 'inner' where outer middlewares wrap inner.
                var router = new Router(routes, new IRouterMiddleware[] { errors, media });

                // manually adding the pingpongr router to the /api path
                api.UsePingPongr(router);
            });


            // the index.html is set to copy with the project
            // and host from the root

            var files = new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory(), "web"));

            app.UseDefaultFiles(new DefaultFilesOptions()
            {
                FileProvider = files,
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = files
            });
        }
    }
}
