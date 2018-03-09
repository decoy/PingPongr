# PingPongr [![Build status](https://ci.appveyor.com/api/projects/status/wl16eoibd2i97a8i/branch/master?svg=true)](https://ci.appveyor.com/project/decoy/pingpongr/branch/master) [![NuGet](https://img.shields.io/nuget/v/PingPongr.svg)](https://www.nuget.org/packages/PingPongr)

A lightweight web service framework.

The framework itself is extremely lean, with minimal dependencies and most of the configuration burden pushed down to the IoC container and other middlwares.  This allows for more flexibility when infrastructure concerns change without having to change your own business logic.

The core router concept was originally based off the infinitely useful [MediatR](https://github.com/jbogard/MediatR) by Jimmy Bogard.  See his blog post  ["Tackling cross-cutting concerns with a mediator pipeline"](https://lostechies.com/jimmybogard/2014/09/09/tackling-cross-cutting-concerns-with-a-mediator-pipeline/) for more information on using this pattern.


## Getting Started

To get started with the common defaults, install these nuget packages.  This includes the "core" PingPongr abstractions, extensions for working with AspNetCore's hosting, and a default JSON serializer.

    Install-Package PingPongr
    Install-Package PingPongr.Extensions.AspNetCore
    Install-Package PingPongr.Serialization.JsonNet

### Example

The basic API consists of the request, the response, and the core handler that processes the request.

Here's a fully functional and self hosted example.

```C#
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
```

In this default example, sending `{ Message: 'Hello' }` to `http//:localhost:5000/Examples/Simple/GetPongFromPing`, would result in the response `{ Reply: 're: Hello'}`.

**Check out the Examples.Complex project to see a more fleshed out sample with custom routes, logging decorators and service injection.**