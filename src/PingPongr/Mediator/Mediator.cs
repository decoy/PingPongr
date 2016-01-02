namespace PingPongr.Mediator
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Collections.Concurrent;

    /// <summary>
    /// Reflection based mediation with pretty syntax sending. 
    /// Obviously originated from https://github.com/jbogard/MediatR
    /// 
    /// Most configuration magic happens at the container rather than extra interface types.
    /// Acts as a wrapper and generator for <see cref="RequestMediator{TRequest, TResponse}"/>.
    /// 
    /// Generated wrappers are cached in a static, concurrent dictionary for faster creation.
    /// </summary>
    public class Mediator : IMediator
    {
        static ConcurrentDictionary<Type, object> typeCache = new ConcurrentDictionary<Type, object>();

        private InstanceFactory factory;

        private MultiInstanceFactory multiFactory;

        public Mediator(InstanceFactory factory, MultiInstanceFactory multiFactory)
        {
            this.factory = factory;
            this.multiFactory = multiFactory;
        }

        public TResponse Send<TResponse>(IRequest<TResponse> request)
        {
            return GetWrapperFromRequest(request).Send(factory, request);
        }

        public Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
        {
            return GetWrapperFromRequest(request).SendAsync(factory, request);
        }

        public Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken)
        {
            return GetWrapperFromRequest(request).SendAsync(factory, request, cancellationToken);
        }

        public IEnumerable<TResponse> Publish<TResponse>(IRequest<TResponse> request)
        {
            return GetWrapperFromRequest(request).Publish(multiFactory, request);
        }

        public IEnumerable<Task<TResponse>> PublishAsync<TResponse>(IRequest<TResponse> request)
        {
            return GetWrapperFromRequest(request).PublishAsync(multiFactory, request);
        }

        public IEnumerable<Task<TResponse>> PublishAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken)
        {
            return GetWrapperFromRequest(request).PublishAsync(multiFactory, request, cancellationToken);
        }

        private static IRequestMediator<TResponse> GetWrapperFromRequest<TResponse>(IRequest<TResponse> request)
        {
            var type = request.GetType();

            //pull from the cache if it's in there, otherwise create and add it
            return (IRequestMediator<TResponse>)typeCache
                .GetOrAdd(type, CreateWrapper<TResponse>);
        }

        private static object CreateWrapper<TResponse>(Type requestType)
        {
            var wrapperType = typeof(RequestMediator<,>)
               .MakeGenericType(requestType, typeof(TResponse));

            return Activator.CreateInstance(wrapperType);
        }
    }
}
