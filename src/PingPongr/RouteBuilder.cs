namespace PingPongr
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Helper for generating the <see cref="Route{TRequest, TResponse}"/> wrappers.
    /// </summary>
    public class RouteBuilder
    {
        private class RouteType
        {
            public Type RequestType { get; }
            public Type ReponseType { get; }

            public RouteType(Type requestType, Type responseType)
            {
                RequestType = requestType;
                ReponseType = responseType;
            }
        }

        private IEnumerable<RouteType> types;

        private Func<RouteType, string> pathBuilder;

        /// <summary>
        /// Creates a new builder from all currently loaded assemblies in the app domain
        /// with the the default filters and path functions
        /// </summary>
        /// <returns></returns>
        public static RouteBuilder WithLoadedAssemblies()
        {
            return new RouteBuilder(AppDomain.CurrentDomain.GetAssemblies());
        }

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
            pathBuilder = t => "/" + t.RequestType.FullName.Replace(".", "/");

            types = GetRouteTypes(assemblies.SelectMany(s => s.ExportedTypes));
        }

        /// <summary>
        /// Filter out types from the specified assemblies.
        /// only possible <see cref="IRouteRequest{TResponse}"/> types will be in the list.
        /// </summary>
        /// <param name="filter">the filter function (where clause)</param>
        /// <returns>this</returns>
        public RouteBuilder WithFilter(Func<Type, bool> filter)
        {
            types = types.Where(t => filter(t.RequestType));
            return this;
        }

        /// <summary>
        /// Filter out request types from the specified assemblies.
        /// Only possible <see cref="IRouteRequest{TResponse}"/> types will be in the list.
        /// </summary>
        /// <param name="filter">the filter function (where clause).  First type is the Request type, second is Response type</param>
        /// <returns>this</returns>
        public RouteBuilder WithFilter(Func<Type, Type, bool> filter)
        {
            types = types.Where(t => filter(t.RequestType, t.ReponseType));
            return this;
        }

        /// <summary>
        /// Overrides the default path building function. 
        /// </summary>
        /// <param name="pathBuilder">Takes in a request type and returns the string value for the path.</param>
        /// <returns>this</returns>
        public RouteBuilder WithPathBuilder(Func<Type, string> pathBuilder)
        {
            this.pathBuilder = t => pathBuilder(t.RequestType);
            return this;
        }

        /// <summary>
        /// Overrides the default path building function. 
        /// </summary>
        /// <param name="pathBuilder">Takes in a request type and response type and returns the string value for the path.</param>
        /// <returns>this</returns>
        public RouteBuilder WithPathBuilder(Func<Type, Type, string> pathBuilder)
        {
            this.pathBuilder = t => pathBuilder(t.RequestType, t.ReponseType);
            return this;
        }

        /// <summary>
        /// Generates the routes from the builder
        /// </summary>
        /// <returns>an enumerable of the routes</returns>
        public IEnumerable<IRoute> GetRoutes()
        {
            return types
                .Select(t => FromType(t, pathBuilder(t)))
                .ToList(); //force the resolution now - fail faster.
        }

        private static bool IsRequest(Type type)
        {
            var info = type.GetTypeInfo();

            return !info.IsAbstract && !info.IsInterface && info.ImplementedInterfaces
                .Any(t => t.GetTypeInfo().IsGenericType && t.GetGenericTypeDefinition() == typeof(IRouteRequest<>));
        }

        private static IRoute FromType(RouteType routeType, string path)
        {
            var type = typeof(Route<,>)
                .MakeGenericType(routeType.RequestType, routeType.ReponseType);

            return (IRoute)Activator.CreateInstance(type, path);
        }

        private static IEnumerable<RouteType> GetRouteTypes(IEnumerable<Type> types)
        {
            // filter out requests that aren't implemented
            foreach (var type in types.Where(t => !t.IsAbstract && !t.IsInterface && !t.IsGenericType))
            {
                // From the implementations, get the response types they implement
                // Note:  It's possible for an individual request to have multiple response types.
                // This requires the paths to be unique to actually work.
                var handlers = type.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRouteRequest<>));

                // from the implemented handlers, get the important type infos
                foreach (var h in handlers)
                {
                    // we know there is one here because we are only looking at items that implement the <> above
                    var args = h.GenericTypeArguments;

                    yield return new RouteType(type, args[0]);
                }
            }
        }
    }
}
