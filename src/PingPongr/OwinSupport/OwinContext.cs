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

        public string RoutePrefix { get; private set; }
        public string RequestPath { get; private set; }
        public string Method { get; private set; }

        public string Path { get; private set; }

        public IDictionary<string, string[]> RequestHeaders { get; private set; }
        public Stream RequestBody { get; private set; }

        public IDictionary<string, string[]> ResponseHeaders { get; private set; }
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

        public string RequestMediaType
        {
            get
            {
                return GetHeaderValues(RequestHeaders, CONTENT_TYPE, string.Empty).First();
            }
        }

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

        public int ContentLength
        {
            get
            {
                var len = GetHeaderValues(ResponseHeaders, CONTENT_LENGTH, "0").First();
                int res = 0;
                if (len != null && int.TryParse(len, out res))
                {
                    return res;
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                ResponseHeaders[CONTENT_LENGTH] = new[] { value.ToString() };
            }
        }

        public CancellationToken CancellationToken { get; private set; }

        public OwinContext(IDictionary<string, object> env, string routePrefix)
        {
            environment = env;
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
