namespace PingPongr.OwinSupport
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class OwinContext : IRequestContext
    {
        private IDictionary<string, object> environment;

        public bool IsHandled { get; set; }

        public string RoutePrefix { get; private set; }
        public string RequestPath { get; private set; }
        public string Method { get; private set; }

        public string Path { get; private set; }
        

        public IDictionary<string, string[]> RequestHeaders { get; private set; }
        public Stream RequestBody { get; private set; }

        public IDictionary<string, string[]> ResponseHeaders { get; private set; }
        public Stream ResponseBody { get; private set; }

        public int StatusCode
        {
            get { return (int)environment[OwinKeys.StatusCode]; }
            set { environment[OwinKeys.StatusCode] = value; }
        }

        public string RequestMediaType
        {
            get { return RequestHeaders["Content-Type"].FirstOrDefault(); }
        }

        public IEnumerable<string> ResponseMediaTypes
        {
            get { return ResponseHeaders["Content-Type"]; }
            set { ResponseHeaders["Content-Type"] = value.ToArray(); }
        }

        public OwinContext(IDictionary<string, object> env, string routePrefix)
        {
            environment = env;

            RoutePrefix = routePrefix;

            RequestPath = (string)env[OwinKeys.RequestPath];

            //the path should not include any routing prefix
            Path = RequestPath;
            if (routePrefix != null && RequestPath.StartsWith(routePrefix))
            {
                Path = Path.Substring(RoutePrefix.Length);
            }

            Method = (string)env[OwinKeys.RequestMethod];

            RequestHeaders = (IDictionary<string, string[]>)env[OwinKeys.RequestHeaders];
            RequestBody = (Stream)env[OwinKeys.RequestBody];

            ResponseHeaders = (IDictionary<string, string[]>)env[OwinKeys.ResponseHeaders];
            ResponseBody = (Stream)env[OwinKeys.ResponseBody];
        }
    }
}
