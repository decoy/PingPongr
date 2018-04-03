namespace PingPongr
{
    using System.Threading.Tasks;

    /// <summary>
    /// The route wrapper implementation.
    /// A cached (stateless) wrapper for running requests of a specific type
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class Route<TRequest, TResponse> : IRoute
       where TRequest : IRouteRequest<TResponse>
    {
        /// <summary>
        /// The path to be matche
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path"></param>
        public Route(string path)
        {
            Path = path;
        }

        /// <summary>
        /// Deserializes the request, resolve the <see cref="IRouteRequestHandler{TRequest, TResponse}"/>, then writes the response
        /// </summary>
        /// <param name="mediaHandler">Media handler for the request</param>
        /// <param name="context">request context</param>
        /// <returns>the awaitable task representing the routing operation</returns>
        public async Task Send(IMediaTypeHandler mediaHandler, IRequestContext context)
        {
            var req = await mediaHandler.Read<TRequest>(context);

            var handler = context.GetService<IRouteRequestHandler<TRequest, TResponse>>();

            var resp = await handler.Handle(req, context.CancellationToken);

            await mediaHandler.Write(context, resp);
        }
    }
}
