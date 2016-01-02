namespace PingPongr.Mediator
{
    /// <summary>
    /// Defines a basic request handler
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public interface IRequestHandler<in TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        /// <summary>
        /// handles the request
        /// </summary>
        /// <param name="message">the request message</param>
        /// <returns>a response</returns>
        TResponse Handle(TRequest message);
    }
}
