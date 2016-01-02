namespace PingPongr.Mediator
{
    /// <summary>
    /// A basic request that expects a response
    /// </summary>
    /// <typeparam name="TResponse">The response type</typeparam>
    public interface IRequest<out TResponse> { }
}
