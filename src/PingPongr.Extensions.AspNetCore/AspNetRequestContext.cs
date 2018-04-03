namespace PingPongr.Extensions.AspNetCore
{
    using Microsoft.AspNetCore.Http;
    using System.IO;
    using System.Threading;

    /// <summary>
    /// Wraps a standard <see cref="HttpContext"/> to be used in PingPongr
    /// </summary>
    public class AspNetRequestContext : IRequestContext
    {
        private readonly HttpContext context;

        /// <summary>
        /// Creates a <see cref="IRequestContext"/> from <see cref="HttpContext"/>
        /// </summary>
        /// <param name="context"></param>
        public AspNetRequestContext(HttpContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Request.Path
        /// </summary>
        public string Path
        {
            get { return context.Request.Path; }
        }

        /// <summary>
        /// If true, status code will be 200.  Else 404.
        /// </summary>
        public bool IsHandled
        {
            get { return context.Response.StatusCode == 200; }
            set { context.Response.StatusCode = value ? 200 : 404; }
        }

        /// <summary>
        /// Request.ContentType
        /// </summary>
        public string RequestContentType
        {
            get { return context.Request.ContentType; }
        }

        /// <summary>
        /// Request.Body
        /// </summary>
        public Stream RequestBody
        {
            get { return context.Request.Body; }
        }

        /// <summary>
        /// Response.ContentType
        /// </summary>
        public string ResponseContentType
        {
            get { return context.Response.ContentType; }
            set { context.Response.ContentType = value; }
        }

        /// <summary>
        /// Response.Body
        /// </summary>
        public Stream ResponseBody
        {
            get { return context.Response.Body; }
        }

        /// <summary>
        /// RequestAborted from the context
        /// </summary>
        public CancellationToken CancellationToken
        {
            get { return context.RequestAborted; }
        }

        /// <summary>
        /// Uses the <see cref="HttpContext.RequestServices"/> to resolve a service type.
        /// Will be scoped for the current request.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetService<T>()
        {
            return (T)context.RequestServices.GetService(typeof(T));
        }
    }
}
