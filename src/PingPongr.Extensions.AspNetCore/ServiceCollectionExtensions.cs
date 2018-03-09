namespace PingPongr
{
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Helpers for registering the Route Handlers against the IServiceCollection 
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers <see cref="IRouteHandler{TRequest, TResponse}"/> for all loaded assemblies as scoped.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddRouteHandlers(this IServiceCollection services)
        {
            return AddRouteHandlers(services, AppDomain.CurrentDomain.GetAssemblies());
        }

        /// <summary>
        /// Registers <see cref="IRouteHandler{TRequest, TResponse}"/> for all the specified assemblies as scoped.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assemblies">assemblies to be scanned for <see cref="IRouteHandler{TRequest, TResponse}"/></param>
        /// <returns></returns>
        public static IServiceCollection AddRouteHandlers(this IServiceCollection services, IEnumerable<Assembly> assemblies)
        {
            return AddRouteHandlers(services, assemblies.SelectMany(s => s.ExportedTypes));
        }

        /// <summary>
        /// Registers route handlers for all the specified types as scoped.
        /// Note:  If the type doesn't implement a handler, it won't be registered.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public static IServiceCollection AddRouteHandlers(this IServiceCollection services, IEnumerable<Type> types)
        {
            foreach (var t in types.Where(t => !t.IsAbstract && !t.IsInterface && !t.IsGenericType))
            {
                AddRouteHandler(services, t);
            }
            return services;
        }

        /// <summary>
        /// Registers route handlers for all the specified type as scoped.
        /// Note:  If the type doesn't implement a handler, it won't be registered.
        /// </summary>
        /// <typeparam name="T">The type of route handler to register</typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddRouteHandler<T>(this IServiceCollection services)
        {
            return AddRouteHandler(services, typeof(T));
        }

        /// <summary>
        /// Registers route handlers for all the specified type as scoped.
        /// Note:  If the type doesn't implement a handler, it won't be registered.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IServiceCollection AddRouteHandler(this IServiceCollection services, Type type)
        {
            foreach (var handler in GetRouteHandlersForType(type))
            {
                services.AddScoped(handler, type);
            }
            return services;
        }

        private static IEnumerable<Type> GetRouteHandlersForType(Type type)
        {
            return type
                .GetTypeInfo()
                .ImplementedInterfaces
                .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IRouteHandler<,>));
        }
    }
}
