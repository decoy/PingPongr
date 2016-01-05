namespace PingPongr
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;

    /// <summary>
    /// Defines a route request
    /// </summary>
    public interface IRequestContext
    {
        /// <summary>
        /// The path to be resolved
        /// </summary>
        string Path { get; }

        /// <summary>
        /// True if this request was processed by a handler (matched a route)
        /// </summary>
        bool IsHandled { get; set; }

        /// <summary>
        /// The request body stream
        /// </summary>
        Stream RequestBody { get; }

        /// <summary>
        /// The request body stream media type (json/xml/etc.)
        /// </summary>
        string RequestMediaType { get; }

        /// <summary>
        /// The response body stream
        /// </summary>
        Stream ResponseBody { get; }

        /// <summary>
        /// Cancellation token for the request
        /// </summary>
        CancellationToken CancellationToken { get; }

        /// <summary>
        /// List of the response body stream medias
        /// Set by the media handlers
        /// </summary>
        IEnumerable<string> ResponseMediaTypes { get; set; }

    }
}