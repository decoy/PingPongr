namespace PingPongr
{
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the interface for routing a request
    /// </summary>
    public interface IRouter
    {
        /// <summary>
        /// Route the request
        /// </summary>
        /// <param name="context">the request context</param>
        /// <returns>An awaitable task that specifies if the route was handled or not</returns>
        Task<bool> RouteRequest(IRequestContext context);
    }
}