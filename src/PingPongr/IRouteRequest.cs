namespace PingPongr
{
    /// <summary>
    /// A basic request that expects a response
    /// </summary>
    /// <typeparam name="TResponse">The response type</typeparam>
    public interface IRouteRequest<out TResponse> { }
}
