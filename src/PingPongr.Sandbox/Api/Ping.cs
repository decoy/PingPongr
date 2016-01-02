namespace PingPongr.Sandbox.Api
{
    public class Ping : Mediator.IRequest<Pong>
    {
        public string Hi { get; set; }
    }
}
