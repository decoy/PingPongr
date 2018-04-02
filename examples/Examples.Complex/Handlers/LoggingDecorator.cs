using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PingPongr;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Examples.Complex.Handlers
{
    public class LoggingDecorator<TRequest, TResponse> : IRouteRequestHandler<TRequest, TResponse>
        where TRequest : IRouteRequest<TResponse>
    {
        private IRouteRequestHandler<TRequest, TResponse> inner;
        private IHttpContextAccessor context;
        private ILogger log;

        public LoggingDecorator(IRouteRequestHandler<TRequest, TResponse> inner, IHttpContextAccessor context, ILogger<LoggingDecorator<TRequest, TResponse>> log)
        {
            this.inner = inner;
            this.context = context;
            this.log = log;
        }

        public async Task<TResponse> Handle(TRequest message, CancellationToken cancellationToken)
        {
            var sw = Stopwatch.StartNew();
            // run the actual decorated item
            var results = await inner.Handle(message, cancellationToken);
            sw.Stop();

            // then log the results!
            log.LogInformation(
                "Processed {message} in {elapsed}ms from path {path} for IP {ip}",
                message.ToString(),
                sw.ElapsedMilliseconds,
                context.HttpContext.Request.Path,
                context.HttpContext.Connection.RemoteIpAddress
                );

            return results;
        }
    }
}
