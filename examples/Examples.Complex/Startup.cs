using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PingPongr;
using PingPongr.Serialization.JsonNet;
using Microsoft.Extensions.FileProviders;
using System.IO;

namespace Examples.Complex
{
    using Services;
    using Handlers;

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
            services.Decorate(typeof(IRouteHandler<,>), typeof(LoggingDecorator<,>));
        }

        public void Configure(IApplicationBuilder app)
        {
            app.Map("/api", api =>
            {
                // overriding the default route path
                var routes = RouteBuilder
                    .WithLoadedAssemblies()
                    .SetPathBuilder(t => "/" + t.Name.ToLower())
                    .GetRoutes();

                // customizing the json settings
                var settings = new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };

                var serializer = JsonSerializer.Create(settings);

                var media = new JsonNetMediaHandler(serializer);

                var router = new Router(routes, new[] { media });

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
