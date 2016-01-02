namespace PingPongr
{
    using Mediator;
    using System.Threading.Tasks;

    public class Route<TRequest, TResponse>
        : RequestMediator<TRequest, TResponse>, IRoute
        where TRequest : IRequest<TResponse>
    {
        public string Path { get; set; }

        public async Task Send(IMediaTypeHandler mediaHandler, InstanceFactory factory, IRequestContext context)
        {
            var req = await mediaHandler.Read<TRequest>(context.RequestBody);
            var resp = await SendAsync(factory, req);
            await mediaHandler.Write(resp, context.ResponseBody, context);
        }
    }
}
