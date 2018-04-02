namespace Examples.Complex.Api
{
    using PingPongr;

    public class Ping : IRouteRequest<Pong>
    {
        public string Name { get; set; }
    }
}
