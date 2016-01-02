namespace PingPongr.Mediator
{
    using System.Threading.Tasks;

    /// <summary>
    /// Defines an asynchronous handler for a request
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public interface IRequestAsyncHandler<in TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        /// <summary>
        /// Asynchronously handles a request
        /// </summary>
        /// <param name="message">the request object</param>
        /// <returns>the response</returns>
        Task<TResponse> Handle(TRequest message);
    }
}
