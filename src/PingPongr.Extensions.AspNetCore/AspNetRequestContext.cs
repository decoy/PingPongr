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
        /// <summary>
        /// Creates a <see cref="IRequestContext"/> from <see cref="HttpContext"/>
        /// </summary>
        /// <param name="context"></param>
        public AspNetRequestContext(HttpContext context)
        {
            Context = context;
        }

        /// <summary>
        /// The <see cref="HttpContext"/> associated with this request.
        /// </summary>
        public HttpContext Context { get; private set; }

        /// <summary>
        /// Request.Path
        /// </summary>
        public string Path
        {
            get { return Context.Request.Path; }
        }

        /// <summary>
        /// Request.ContentType
        /// </summary>
        public string RequestContentType
        {
            get { return Context.Request.ContentType; }
        }

        /// <summary>
        /// Request.Body
        /// </summary>
        public Stream RequestBody
        {
            get { return Context.Request.Body; }
        }

        /// <summary>
        /// Response.ContentType
        /// </summary>
        public string ResponseContentType
        {
            get { return Context.Response.ContentType; }
            set { Context.Response.ContentType = value; }
        }

        /// <summary>
        /// Response.Body
        /// </summary>
        public Stream ResponseBody
        {
            get { return Context.Response.Body; }
        }

        /// <summary>
        /// RequestAborted from the context
        /// </summary>
        public CancellationToken CancellationToken
        {
            get { return Context.RequestAborted; }
        }

        /// <summary>
        /// Uses the <see cref="HttpContext.RequestServices"/> to resolve a service type.
        /// Will be scoped for the current request.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetService<T>()
        {
            return (T)Context.RequestServices.GetService(typeof(T));
        }
    }
}
