namespace Examples.Simple
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using PingPongr;
    using PingPongr.Serialization.JsonNet;
    using System.Threading;
    using System.Threading.Tasks;

    // Request and Response objects are just simple objects that can be shared between routes.
    public class Ping
    {
        public string Message { get; set; }
    }

    public class Pong
    {
        public string Reply { get; set; }
    }

    // A handler processes a request and returns a response.
    public class GetPongFromPing : IRouteHandler<Ping, Pong>
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
