namespace PingPongr.Sandbox.Api
{
    public class Ping : IRouteRequest<Pong>
    {
        public string Hi { get; set; }
    }
}
