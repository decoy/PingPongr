namespace PingPongr.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Linq;

    public class FakeRequest : IRequestContext, IDisposable
    {
        private Dictionary<Type, object> Services { get; set; } = new Dictionary<Type, object>();

        public string Path { get; set; }

        public Stream RequestBody { get; set; } = new MemoryStream();

        public string RequestContentType { get; set; }

        public Stream ResponseBody { get; set; } = new MemoryStream();

        public string ResponseContentType { get; set; }

        public CancellationToken CancellationToken { get; set; } = CancellationToken.None;

        public T GetService<T>()
        {
            var t = typeof(T);
            if (Services.ContainsKey(t))
            {
                return (T)Services[t];
            }

            throw new ArgumentException("Unknown service: " + t.FullName);
        }

        public void Dispose()
        {
            RequestBody.Dispose();
            ResponseBody.Dispose();
        }

        /// <summary>
        /// Quick helper for 'registering' a handler with behaviors
        /// </summary>
        public void AddHandlerService<TRequest, TResponse>(
            IRouteRequestHandler<TRequest, TResponse> handler,
            IEnumerable<IRouteRequestBehavior<TRequest, TResponse>> behaviors = null)
            where TRequest : IRouteRequest<TResponse>
        {
            Services.Add(typeof(IRouteRequestHandler<TRequest, TResponse>), handler);

            var empty = Enumerable.Empty<IRouteRequestBehavior<TRequest, TResponse>>();
            var type = typeof(IEnumerable<IRouteRequestBehavior<TRequest, TResponse>>);

            if (behaviors == null)
            {
                Services.Add(type, empty);
            }
            else
            {
                Services.Add(type, behaviors);
            }
        }
    }
}
