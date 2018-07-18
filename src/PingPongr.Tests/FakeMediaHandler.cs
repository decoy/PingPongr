namespace PingPongr.Tests
{
    using System.Threading.Tasks;

    public class FakeMedia : IRouterMiddleware
    {
        public bool HasRead { get; private set; }

        public bool HasWritten { get; private set; }

        public bool CanHandle(string contentType)
        {
            return true;
        }

        public Task<T> Read<T>(IRequestContext context)
        {
            HasRead = true;
            return Task.FromResult(default(T));
        }

        public Task Write<T>(IRequestContext context, T content)
        {
            HasWritten = true;
            return Task.CompletedTask;
        }

        public async Task<TResponse> Route<TRequest, TResponse>(IRequestContext context, RequestHandlerDelegate<TRequest, TResponse> route, RouteMiddlewareDelegate<TResponse> next)
        {
            if (CanHandle(context.RequestContentType))
            {
                var req = await Read<TRequest>(context);

                var resp = await route(req, context.CancellationToken);

                await Write(context, resp);

                context.IsHandled = true;

                return resp;
            }
            else
            {
                return await next();
            }
        }
    }
}
