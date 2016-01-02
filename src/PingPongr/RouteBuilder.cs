namespace PingPongr
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Mediator;

    /// <summary>
    /// Helper for generating the <see cref="Route{TRequest, TResponse}"/> wrappers.
    /// </summary>
    public class RouteBuilder
    {
        private IEnumerable<Type> types { get; set; }
        private Func<Type, string> pathBuilder { get; set; }

        /// <summary>
        /// Creates a new builder from the specified assembles
        /// with the the default filters and path functions
        /// </summary>
        /// <param name="assemblies"></param>
        public static RouteBuilder WithAssemblies(IEnumerable<Assembly> assemblies)
        {
            return new RouteBuilder(assemblies);
        }

        /// <summary>
        /// Creates a new builder from the specified assembles
        /// with the the default filters and path functions
        /// </summary>
        /// <param name="assemblies"></param>
        public static RouteBuilder WithAssemblies(params Assembly[] assemblies)
        {
            return new RouteBuilder(assemblies);
        }

        /// <summary>
        /// Creates a new builder from the specified assembles
        /// with the the default filters and path functions
        /// </summary>
        /// <param name="assemblies"></param>
        public RouteBuilder(IEnumerable<Assembly> assemblies)
        {
            //defaults
            pathBuilder = t => "/" + t.FullName.Replace(".", "/");

            this.types = assemblies.SelectMany(s => s.GetTypes())
              .Where(IsRequest);
        }

        /// <summary>
        /// Filter out types from the specified assemblies.
        /// only possible <see cref="IRequest{TResponse}"/> types will be in the list.
        /// </summary>
        /// <param name="filter">the filter function (where clause)</param>
        /// <returns>this</returns>
        public RouteBuilder Filter(Func<Type, bool> filter)
        {
            types = types.Where(filter);
            return this;
        }

        /// <summary>
        /// Allows for overriding the default path.
        /// Default: t => "/" + t.FullName.Replace(".", "/");
        /// </summary>
        /// <param name="pathBuilder">the function to build paths from the type</param>
        /// <returns>this</returns>
        public RouteBuilder Path(Func<Type, string> pathBuilder)
        {
            this.pathBuilder = pathBuilder;
            return this;
        }

        /// <summary>
        /// Generates the routes from the builder
        /// </summary>
        /// <returns>an enumerable of the routes</returns>
        public IEnumerable<IRoute> GetRoutes()
        {
            return types.Select(t =>
            {
                var route = FromType(t);
                route.Path = pathBuilder(t);
                return route;
            }).ToList(); //force the resolution now - fail faster.
        }

        private static bool IsRequest(Type type)
        {
            return !type.IsAbstract && !type.IsInterface && type.GetInterfaces()
                .Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IRequest<>));
        }

        private static IRoute FromType(Type requestType)
        {
            var type = typeof(Route<,>)
                .MakeGenericType(requestType, GetResponseType(requestType));

            return (IRoute)Activator.CreateInstance(type);
        }

        /// <summary>
        /// Gets the response type from a <see cref="IRequest{TResponse}"/> type
        /// </summary>
        private static Type GetResponseType(Type requestType)
        {
            return requestType.GetInterfaces()
                .Single(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IRequest<>))
                .GetGenericArguments()
                .Single();
        }
    }


}
