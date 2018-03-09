using PingPongr;
using System.Threading;
using System.Threading.Tasks;

namespace Examples.Complex.Handlers
{
    using Api;
    using Services;

    public class GetGreeting : IRouteHandler<Ping, Pong>
    {
        private GreetingService service;

        public GetGreeting(GreetingService service)
        {
            this.service = service;
        }

        public Task<Pong> Handle(Ping request, CancellationToken cancellationToken)
        {
            var pong = new Pong()
            {
                Reply = service.GetRandomGreeting() + " " + request.Name
            };

            return Task.FromResult(pong);
        }
    }
}
