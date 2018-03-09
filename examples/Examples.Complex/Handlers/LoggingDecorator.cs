using Microsoft.AspNetCore.Http;
using PingPongr;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Examples.Complex.Handlers
{
    public class LoggingDecorator<TRequest, TResponse> : IRouteHandler<TRequest, TResponse>
    {
        IRouteHandler<TRequest, TResponse> inner;
        IHttpContextAccessor context;
        ILogger log;

        public LoggingDecorator(IRouteHandler<TRequest, TResponse> inner, IHttpContextAccessor context, ILogger<LoggingDecorator<TRequest, TResponse>> log)
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

            // the log the results!
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
