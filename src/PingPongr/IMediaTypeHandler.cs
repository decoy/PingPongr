namespace PingPongr
{
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a media handler for incoming and outgoing streams
    /// </summary>
    public interface IMediaTypeHandler
    {
        /// <summary>
        /// A function to see if this handler can process the specified media type.
        /// ex: "application/json"
        /// </summary>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        bool CanHandle(string contentType);

        /// <summary>
        /// Reads from the input stream
        /// </summary>
        /// <typeparam name="T">The type to be deserialized from the stream</typeparam>
        /// <param name="context">the request context with access to headers, response body and cancellation tokens</param>
        /// <returns>an awaitable task with the results of the deserialized stream</returns>
        Task<T> Read<T>(IRequestContext context);

        /// <summary>
        /// Writes an object to the output stream 
        /// </summary>
        /// <param name="context">the request context with access to headers, response body and cancellation tokens</param>
        /// <param name="content">the object to be written</param>
        /// <returns>An awaitable taks</returns>
        Task Write<T>(IRequestContext context, T content);
    }
}
