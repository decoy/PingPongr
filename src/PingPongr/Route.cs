namespace PingPongr
{
    using System.Threading.Tasks;
    using System.Threading;

    /// <summary>
    /// The route wrapper implementation.
    /// A cached (stateless) wrapper for running requests of a specific type
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class Route<TRequest, TResponse> : IRoute
        where TRequest : IRouteRequest<TResponse>
    {
        public string Path { get; set; }

        /// <summary>
        /// Deserializes the request, sends it through the mediator as async, then writes the response
        /// </summary>
        /// <param name="mediaHandler">Media handler for the request</param>
        /// <param name="factory">instance factory for generating the handlers</param>
        /// <param name="context">request context</param>
        /// <returns>the awaitable task representing the routing operation</returns>
        public async Task Send(IMediaTypeHandler mediaHandler, InstanceFactory factory, IRequestContext context)
        {
            var req = await mediaHandler.Read<TRequest>(context.RequestBody, context.CancellationToken);
            var resp = await SendAsync(factory, req, context.CancellationToken);
            await mediaHandler.Write(resp, context.ResponseBody, context, context.CancellationToken);
        }

        private static Task<TResponse> SendAsync(InstanceFactory factory, IRouteRequest<TResponse> msg, CancellationToken cancellationToken)
        {
            return factory
                .Resolve<IRouteRequestHandler<TRequest, TResponse>>()
                .Handle((TRequest)msg, cancellationToken);
        }
    }
}
