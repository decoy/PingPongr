namespace Examples.Simple
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using PingPongr;
    using PingPongr.JsonNet;
    using System.Threading;
    using System.Threading.Tasks;

    // A request is unique per route and defines the expected input/output.
    public class Ping : IRouteRequest<Pong>
    {
        public string Message { get; set; }
    }

    // Responses are just simple objects and can be shared between requests.
    public class Pong
    {
        public string Reply { get; set; }
    }

    // A handler processes a request and returns the response.
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

            // Using the PingPongr.JsonNet media handler.
            services.AddSingleton<IMediaTypeHandler, JsonNetMediaHandler>();

            //services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        public void Configure(IApplicationBuilder app)
        {
            // Add PingPongr to the app pipeline.
            app.UsePingPongr();

            // TODO - kill off irequest<>, replace with handlers = route
            // routes should use path matches Func<bool> instead of dictionary
            // route builder and service reg need to be able to deal with nested abstracts
            //  and multiple interfaces attached
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
