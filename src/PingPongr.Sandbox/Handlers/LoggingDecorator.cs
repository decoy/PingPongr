namespace PingPongr.Sandbox.Handlers
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Services;

    public class LoggingDecorator<TRequest, TResponse>
        : IRouteRequestHandler<TRequest, TResponse>
        where TRequest : IRouteRequest<TResponse>
    {
        IRouteRequestHandler<TRequest, TResponse> inner;
        IUserContext context;
        public LoggingDecorator(IRouteRequestHandler<TRequest, TResponse> inner, IUserContext context)
        {
            this.inner = inner;
            this.context = context;
        }

        public async Task<TResponse> Handle(TRequest message, CancellationToken cancellationToken)
        {
            var sw = Stopwatch.StartNew();
            var results = await this.inner.Handle(message, cancellationToken);
            sw.Stop();

            Console.WriteLine(String.Format("Processed {0} in {1}ms from path {2}", message.ToString(), sw.ElapsedMilliseconds, context.RequestPath));

            return results;
        }
    }
}
