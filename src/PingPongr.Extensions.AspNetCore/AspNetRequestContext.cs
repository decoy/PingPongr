namespace PingPongr.Extensions.AspNetCore
{
    using Microsoft.AspNetCore.Http;
    using System.IO;
    using System.Threading;

    /// <summary>
    /// Wraps a standard <see cref="Microsoft.AspNetCore.Http.HttpContext"/> to be used in PingPongr
    /// </summary>
    public class AspNetRequestContext : IRequestContext
    {
        /// <summary>
        /// Creates a <see cref="IRequestContext"/> from <see cref="Microsoft.AspNetCore.Http.HttpContext"/>
        /// </summary>
        /// <param name="context"></param>
        public AspNetRequestContext(HttpContext context)
        {
            HttpContext = context;
        }

        /// <summary>
        /// The <see cref="Microsoft.AspNetCore.Http.HttpContext"/> associated with this request.
        /// </summary>
        public HttpContext HttpContext { get; private set; }

        /// <summary>
        /// Request.Path
        /// </summary>
        public string Path
        {
            get { return HttpContext.Request.Path; }
        }

        /// <summary>
        /// Request.ContentType
        /// </summary>
        public string RequestContentType
        {
            get { return HttpContext.Request.ContentType; }
        }

        /// <summary>
        /// Request.Body
        /// </summary>
        public Stream RequestBody
        {
            get { return HttpContext.Request.Body; }
        }

        /// <summary>
        /// Response.ContentType
        /// </summary>
        public string ResponseContentType
        {
            get { return HttpContext.Response.ContentType; }
            set { HttpContext.Response.ContentType = value; }
        }

        /// <summary>
        /// Response.Body
        /// </summary>
        public Stream ResponseBody
        {
            get { return HttpContext.Response.Body; }
        }

        /// <summary>
        /// RequestAborted from the context
        /// </summary>
        public CancellationToken CancellationToken
        {
            get { return HttpContext.RequestAborted; }
        }

        /// <summary>
        /// Uses the <see cref="HttpContext.RequestServices"/> to resolve a service type.
        /// Will be scoped for the current request.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetService<T>()
        {
            return (T)HttpContext.RequestServices.GetService(typeof(T));
        }
    }
}
