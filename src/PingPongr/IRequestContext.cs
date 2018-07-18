namespace PingPongr
{
    using System.IO;
    using System.Threading;

    /// <summary>
    /// The context of a request.  Generally aligns with the HttpContext.
    /// </summary>
    public interface IRequestContext
    {
        /// <summary>
        /// The path to be resolved
        /// </summary>
        string Path { get; }

        /// <summary>
        /// The request body stream media type (json/xml/etc.)
        /// </summary>
        string RequestContentType { get; }

        /// <summary>
        /// The request body stream
        /// </summary>
        Stream RequestBody { get; }

        /// <summary>
        /// The response media type
        /// </summary>
        string ResponseContentType { get; set; }

        /// <summary>
        /// The response body stream
        /// </summary>
        Stream ResponseBody { get; }

        /// <summary>
        /// Cancellation token for the request
        /// </summary>
        CancellationToken CancellationToken { get; }

        /// <summary>
        /// Gets a service of type T scoped for this request context.
        /// Actual implementation should be part of the DI container.
        /// </summary>
        /// <typeparam name="T">The service type</typeparam>
        /// <returns>A service of type T</returns>
        T GetService<T>();
    }
}
