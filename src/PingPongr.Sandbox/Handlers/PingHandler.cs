namespace PingPongr.Sandbox.Handlers
{
    using Api;
    using System.Threading;
    using System.Threading.Tasks;

    public class PingHandler : IRouteRequestHandler<Ping, Pong>
    {
        public async Task<Pong> Handle(Ping message, CancellationToken cancellationToken)
        {
            return await Task.FromResult(new Pong { Reply = message.Hi + " Pong" });
        }
    }
}
