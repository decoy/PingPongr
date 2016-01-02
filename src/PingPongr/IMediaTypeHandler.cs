namespace PingPongr
{
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a media handler for incoming and outgoing streams
    /// </summary>
    public interface IMediaTypeHandler
    {
        /// <summary>
        /// A function to see if this handler can process the specified media type.
        /// "application/json"
        /// </summary>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        bool CanHandleMediaType(string mediaType);

        /// <summary>
        /// Reads from the input stream
        /// </summary>
        /// <typeparam name="T">The type to be deserialized from the stream</typeparam>
        /// <param name="inputStream">the stream to be read from</param>
        /// <returns>an awaitable task with the results of the deserialized stream</returns>
        Task<T> Read<T>(Stream inputStream);

        /// <summary>
        /// Writes an object to the output stream 
        /// </summary>
        /// <param name="content">the object to be written</param>
        /// <param name="outputStream">the stream to write to</param>
        /// <param name="context">the request context (to set response media types)</param>
        /// <returns>An awaitable taks</returns>
        Task Write(object content, Stream outputStream, IRequestContext context);
    }
}
