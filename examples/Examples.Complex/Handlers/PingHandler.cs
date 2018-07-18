using PingPongr;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Examples.Complex.Handlers
{
    using Api;
    using Services;

    public class PingHandler : IRouteRequestHandler<Ping, Pong>
    {
        private GreetingService service;

        public PingHandler(GreetingService service)
        {
            this.service = service;
        }

        public async Task<Pong> Handle(Ping request, CancellationToken cancellationToken)
        {
            if (request.Name == "Bob")
            {
                // demonstrate the custom exception middleware
                throw new UnauthorizedAccessException("Bob isn't allowed here.");
            }

            var greeting = await service.GetRandomGreeting();

            var pong = new Pong()
            {
                Reply = greeting + " " + request.Name
            };

            return pong;
        }
    }
}
