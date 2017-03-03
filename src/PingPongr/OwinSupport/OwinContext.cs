namespace PingPongr.OwinSupport
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// Wrapper for the Owin context environment dictionary
    /// </summary>
    public class OwinContext : IRequestContext
    {
        const string CONTENT_LENGTH = "Content-Length";
        const string CONTENT_TYPE = "Content-Type";

        private IDictionary<string, object> environment;

        /// <summary>
        /// Wraps the Status Code.
        /// Get returns true if 200, anything else is false.
        /// If set to false, will set status code to 404.
        /// </summary>
        public bool IsHandled
        {
            get
            {
                return StatusCode == 200;
            }
            set
            {
                StatusCode = value ? 200 : 404;
            }
        }

        /// <summary>
        /// Prefix for the incoming request
        /// </summary>
        public string RoutePrefix { get; private set; }

        /// <summary>
        /// The full path of the incoming request
        /// </summary>
        public string RequestPath { get; private set; }

        /// <summary>
        /// What HTTP method the incoming request is
        /// </summary>
        public string Method { get; private set; }

        /// <summary>
        /// The <see cref="RequestPath"/> minus the <see cref="RoutePrefix"/>.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// All incoming http headers
        /// </summary>
        public IDictionary<string, string[]> RequestHeaders { get; private set; }

        /// <summary>
        /// The request body stream
        /// </summary>
        public Stream RequestBody { get; private set; }

        /// <summary>
        /// All outgoing response headers
        /// </summary>
        public IDictionary<string, string[]> ResponseHeaders { get; private set; }

        /// <summary>
        /// THe response body stream
        /// </summary>
        public Stream ResponseBody { get; private set; }

        /// <summary>
        /// The HTTP status code. This is set by IsHandled = true = 200, false = 404
        /// </summary>
        public int StatusCode
        {
            get
            {
                return GetEnvironmentValue(OwinKeys.StatusCode, 0);
            }
            set
            {
                environment[OwinKeys.StatusCode] = value;
            }
        }

        /// <summary>
        /// The media type, pulled from the Request Headers, content type
        /// </summary>
        public string RequestMediaType
        {
            get
            {
                return GetHeaderValues(RequestHeaders, CONTENT_TYPE, string.Empty).First();
            }
        }

        /// <summary>
        /// The response media headers.  Placed in the Content type header.
        /// </summary>
        public IEnumerable<string> ResponseMediaTypes
        {
            get
            {
                return GetHeaderValues(ResponseHeaders, CONTENT_TYPE, string.Empty);
            }
            set
            {
                ResponseHeaders[CONTENT_TYPE] = value.ToArray();
            }
        }

        /// <summary>
        /// Cancellation token from the HTTP host.  
        /// </summary>
        public CancellationToken CancellationToken { get; private set; }

        /// <summary>
        /// Creates an Owin Context from the dictionary of environment data
        /// </summary>
        /// <param name="environment">The environment dictionary from the owin request</param>
        public OwinContext(IDictionary<string, object> environment) : this(environment, null) { }

        /// <summary>
        /// Creates an Owin Context from the dictionary of environment data
        /// </summary>
        /// <param name="environment">The environment dictionary from the owin request</param>
        /// <param name="routePrefix">The route prefix (if any)</param>
        public OwinContext(IDictionary<string, object> environment, string routePrefix)
        {
            if (environment == null)
            {
                throw new ArgumentNullException("env");
            }

            this.environment = environment;
            RoutePrefix = routePrefix;

            RequestPath = GetEnvironmentValue<string>(OwinKeys.RequestPath, null);
            if (RequestPath == null)
            {
                throw new ArgumentNullException(OwinKeys.RequestPath, "The environment key RequestPath cannot be null in order to route.");
            }

            //the path should not include any routing prefix
            Path = RequestPath;
            if (routePrefix != null && RequestPath.StartsWith(routePrefix))
            {
                Path = Path.Substring(RoutePrefix.Length);
            }

            Method = GetEnvironmentValue(OwinKeys.RequestMethod, "GET");

            RequestHeaders = GetEnvironmentValue<IDictionary<string, string[]>>(OwinKeys.RequestHeaders, null);
            if (RequestHeaders == null)
            {
                RequestHeaders = new Dictionary<string, string[]>();
            }

            RequestBody = GetEnvironmentValue<Stream>(OwinKeys.RequestBody, null);
            if (RequestBody == null)
            {
                throw new ArgumentNullException(OwinKeys.RequestBody, "The environment key RequestBody cannot be null.");
            }

            ResponseHeaders = GetEnvironmentValue<IDictionary<string, string[]>>(OwinKeys.ResponseHeaders, null);
            if (ResponseHeaders == null)
            {
                ResponseHeaders = new Dictionary<string, string[]>();
            }

            ResponseBody = GetEnvironmentValue<Stream>(OwinKeys.ResponseBody, null);
            if (ResponseBody == null)
            {
                throw new ArgumentNullException(OwinKeys.ResponseBody, "The environment key ResponseBody cannot be null.");
            }


            CancellationToken = GetEnvironmentValue(OwinKeys.CallCancelled, CancellationToken.None);
        }

        private IEnumerable<string> GetHeaderValues(IDictionary<string, string[]> headers, string key, string defaultValue)
        {
            if (headers == null || !headers.ContainsKey(key))
            {
                yield return defaultValue;
            }

            foreach (var header in headers[key])
            {
                yield return header;
            }
        }

        private T GetEnvironmentValue<T>(string key, T defaultValue)
        {
            if (environment.ContainsKey(key))
            {
                return (T)environment[key];
            }
            else
            {
                return defaultValue;
            }
        }
    }
}
