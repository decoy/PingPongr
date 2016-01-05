namespace PingPongr.Sandbox.Handlers
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    public class LoggingDecorator<TRequest, TResponse>
        : IRouteRequestHandler<TRequest, TResponse>
        where TRequest : IRouteRequest<TResponse>
    {
        IRouteRequestHandler<TRequest, TResponse> inner;
        IOwinContextProvider contextProvider;
        public LoggingDecorator(IRouteRequestHandler<TRequest, TResponse> inner, IOwinContextProvider contextProvider)
        {
            this.inner = inner;
            this.contextProvider = contextProvider;
        }

        public async Task<TResponse> Handle(TRequest message, CancellationToken cancellationToken)
        {
            var sw = Stopwatch.StartNew();
            var results = await this.inner.Handle(message, cancellationToken);
            sw.Stop();

            Console.WriteLine(String.Format("Processed {0} in {1}ms from path {2}", message.ToString(), sw.ElapsedMilliseconds, contextProvider.CurrentContext.Request.Path));

            return results;
        }
    }
}
