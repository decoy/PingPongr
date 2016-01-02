namespace PingPongr.Sandbox.Handlers
{
    using Mediator;
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    public class LoggingDecorator<TRequest, TResponse>
        : IRequestAsyncHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        IRequestAsyncHandler<TRequest, TResponse> inner;
        public LoggingDecorator(IRequestAsyncHandler<TRequest, TResponse> inner)
        {
            this.inner = inner;
        }

        public async Task<TResponse> Handle(TRequest message)
        {
            var sw = Stopwatch.StartNew();
            var results = await this.inner.Handle(message);
            sw.Stop();

            Console.WriteLine(String.Format("Processed {0} in {1}ms", message.ToString(), sw.ElapsedMilliseconds));

            return results;
        }
    }
}
