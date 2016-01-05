# PingPongr [![Build status](https://ci.appveyor.com/api/projects/status/wl16eoibd2i97a8i/branch/master?svg=true)](https://ci.appveyor.com/project/decoy/pingpongr/branch/master) [![NuGet](https://img.shields.io/nuget/v/PingPongr.svg)](https://www.nuget.org/packages/PingPongr)

An experimental, lightweight RPC framework.

The framework itself is extremely lean, with minimal dependencies and most of the configuration burden pushed down to the IoC container and other Owin middlwares.  This allows for more flexibility when infrastructure concerns change without having to change your own business logic.

The core router concept is based off the infinitely useful [MediatR](https://github.com/jbogard/MediatR) by Jimmy Bogard.  See his blog post  ["Tackling cross-cutting concerns with a mediator pipeline"](https://lostechies.com/jimmybogard/2014/09/09/tackling-cross-cutting-concerns-with-a-mediator-pipeline/) for more information on using this pattern.

## Example

Your main API consists of the request, the response, and the core handler that processes the request.

```C#
    using PingPongr;
    using System.Threading;
    using System.Threading.Tasks;
    
    //A request is unique per route
    public class Ping : IRouteRequest<Pong>
    {
        public string Hi { get; set; }
    }
    
    //responses can be shared between routes
    public class Pong
    {
        public string Reply { get; set; }
    }
    
    public class PingHandler : IRouteRequestHandler<Ping, Pong>
    {
        public async Task<Pong> Handle(Ping message, CancellationToken cancellationToken)
        {
            return await DoSomethingCoolAsync();
        }
    }
```

A functional, self hosted example using SimpleInjector

```C#
    using Owin;
    using PingPongr;
    using PingPongr.OwinSupport;
    using SimpleInjector;
    using System;

    public class Program
    {
        public class Startup
        {
            public void Configuration(IAppBuilder app)
            {
                //setup the container
                var container = new Container();
                var assemblies = new[] { typeof(Program).Assembly };

                //register all the route request handlers
                container.Register(typeof(IRouteRequestHandler<,>), assemblies);

                container.Verify();

                //an instance factory is how request handlers are built from the container.
                var factory = new InstanceFactory(container.GetInstance);

                //the lib comes with a json media handler built with SimpleJson
                var mediaHandlers = new[] { new DefaultJsonMediaHandler() };

                //the routes are found using reflection via the RouteBuilder
                var routes = RouteBuilder.WithAssemblies(assemblies).GetRoutes();
                foreach (var r in routes) Console.WriteLine(r.Path);

                //setup the PingPongr router
                //(Note: we're setting this up manually, but it could be created by the container)
                IRouter router = new Router(
                    routes,
                    mediaHandlers,
                    factory
                    );

                app.UsePingPongr(router);
            }
        }

        public static void Main(string[] args)
        {
            using (Microsoft.Owin.Hosting.WebApp.Start<Startup>("http://localhost:12345"))
            {
                Console.ReadLine();
            }
        }
    }
```

Checkout the PingPongr.Sandbox example project to see a more fully formed example including using decorators and injecting the OwinContext as a dependency to a handler.