namespace PingPongr.Sandbox.Handlers
{
    using Api;
    using Mediator;
    using System.Threading.Tasks;

    public class PingHandler : IRequestAsyncHandler<Ping, Pong>
    {
        public async Task<Pong> Handle(Ping message)
        {
            return await Task.Factory.StartNew(() => new Pong { Reply = message.Hi + " Pong" });
        }
    }
}
