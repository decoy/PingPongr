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
        /// <summary>
        /// Wraps the builder types
        /// </summary>
        private class RouteTypes
        {
            /// <summary>
            /// The implementation handler class
            /// </summary>
            public Type Implementation { get; set; }

            /// <summary>
            /// The <see cref="IRouteHandler{TRequest, TResponse}"/> type
            /// </summary>
            public Type RequestHandler { get; set; }

            /// <summary>
            /// The TRequest type
            /// </summary>
            public Type Request { get; set; }

            /// <summary>
            /// The TResponse type
            /// </summary>
            public Type Response { get; set; }
        }

        private IEnumerable<RouteTypes> types;

        private Func<RouteTypes, string> pathBuilder;

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
            //default path builder
            pathBuilder = t => "/" + t.Implementation.FullName.Replace(".", "/");

            types = GetRouteTypes(assemblies.SelectMany(s => s.ExportedTypes));
        }

        /// <summary>
        /// Filter out types from the specified assemblies.
        /// Only concrete <see cref="IRouteHandler{TRequest, TResponse}"/> types will be in the list.
        /// </summary>
        /// <param name="filter">the filter function (where clause)</param>
        /// <returns>this</returns>
        public RouteBuilder Filter(Func<Type, bool> filter)
        {
            types = types.Where(t => filter(t.Implementation));
            return this;
        }

        /// <summary>
        /// Allows for overriding the default route path.
        /// Default: t => "/" + t.FullName.Replace(".", "/");
        /// </summary>
        /// <param name="pathBuilder">the function to build paths from the type</param>
        /// <returns>this</returns>
        public RouteBuilder SetPathBuilder(Func<Type, string> pathBuilder)
        {
            this.pathBuilder = t => pathBuilder(t.Implementation);
            return this;
        }

        /// <summary>
        /// Allows for overriding the default route path.
        /// </summary>
        /// <param name="pathBuilder">function accepting Implementation Type, Request Type, Response Type and returning a string</param>
        /// <returns></returns>
        public RouteBuilder SetPathBuilder(Func<Type, Type, Type, string> pathBuilder)
        {
            this.pathBuilder = t => pathBuilder(t.Implementation, t.Request, t.Response);
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
                var route = FromRouteTypes(t);
                route.Path = pathBuilder(t);
                return route;
            }).ToList(); //force the resolution now - fail faster.
        }

        private static IRoute FromRouteTypes(RouteTypes route)
        {
            var type = typeof(Route<,>)
                .MakeGenericType(route.Request, route.Response);

            // not the best way to create a class, but 'good enough' for building them on first load
            return (IRoute)Activator.CreateInstance(type);
        }

        private static IEnumerable<RouteTypes> GetRouteTypes(IEnumerable<Type> types)
        {
            // filter out handlers that aren't implemented
            foreach (var type in types.Where(t => !t.IsAbstract && !t.IsInterface && !t.IsGenericType))
            {
                // from the implementations, get the generic route request handlers they implement.
                // an individual handler can implement multiple routes if it wants (will break default paths, though)
                var handlers = type.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRouteHandler<,>));

                // from the implemented handlers, get the important type infos
                foreach (var h in handlers)
                {
                    // we know there are two because they're items that implement the <,> above
                    var args = h.GenericTypeArguments;

                    yield return new RouteTypes()
                    {
                        Implementation = type,
                        Request = args[0],
                        Response = args[1],
                        RequestHandler = h
                    };
                }
            }
        }
    }


}
