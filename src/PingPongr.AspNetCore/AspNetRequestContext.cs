namespace PingPongr.AspNetCore
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

        public AspNetRequestContext(HttpContext context)
        {
            this.context = context;
        }

        public string Path
        {
            get { return context.Request.Path; }
        }

        public bool IsHandled
        {
            get { return context.Response.StatusCode == 200; }
            set { context.Response.StatusCode = value ? 200 : 404; }
        }

        public string RequestContentType
        {
            get { return context.Request.ContentType; }
        }

        public Stream RequestBody
        {
            get { return context.Request.Body; }
        }

        public string ResponseContentType
        {
            get { return context.Response.ContentType; }
            set { context.Response.ContentType = value; }
        }

        public Stream ResponseBody
        {
            get { return context.Response.Body; }
        }

        public CancellationToken CancellationToken
        {
            get { return context.RequestAborted; }
        }

        public T GetService<T>()
        {
            return (T)context.RequestServices.GetService(typeof(T));
        }
    }
}
