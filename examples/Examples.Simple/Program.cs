namespace Examples.Simple
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using PingPongr;
    using PingPongr.Serialization.JsonNet;
    using System.Threading;
    using System.Threading.Tasks;

    // A request is unique per route.
    // It defines what response type is expected.
    public class Ping : IRouteRequest<Pong>
    {
        public string Message { get; set; }
    }

    // Response types can be shared between requests.
    public class Pong
    {
        public string Reply { get; set; }
    }

    // A handler processes a request and returns a response.
    public class PingHandler : IRouteRequestHandler<Ping, Pong>
    {
        public Task<Pong> Handle(Ping request, CancellationToken cancellationToken)
        {
            // Do something cool here. (This example is not cool.  Be cooler.  Be amazing.)
            return Task.FromResult(new Pong() { Reply = "re: " + request.Message });
        }
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // add the route handlers to the service registrations.
            services.AddRouteHandlers();

            // Using the PingPongr.JsonNet media handler middleware
            services.AddSingleton<IRouterMiddleware, JsonNetMiddleware>();
        }

        public void Configure(IApplicationBuilder app)
        {
            // Add PingPongr to the app pipeline.
            app.UsePingPongr();

            // By default, the endpoint will be available at:  Examples/Simple/Ping
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) => new WebHostBuilder()
                .UseKestrel()
                .UseStartup<Startup>()
                .Build();
    }
}
