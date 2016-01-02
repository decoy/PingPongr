namespace PingPongr
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Mediator;

    /// <summary>
    /// Gets routes from possible types
    /// </summary>
    public class RouteBuilder
    {
        private IEnumerable<Type> types { get; set; }
        private Func<Type, string> pathBuilder { get; set; }

        public static RouteBuilder WithAssemblies(IEnumerable<Assembly> assemblies)
        {
            return new RouteBuilder(assemblies);
        }

        public static RouteBuilder WithAssemblies(params Assembly[] assemblies)
        {
            return new RouteBuilder(assemblies);
        }

        public static RouteBuilder WithLoadedAssemblies()
        {
            return new RouteBuilder(AppDomain.CurrentDomain.GetAssemblies());
        }

        public RouteBuilder(IEnumerable<Assembly> assemblies)
        {
            //defaults
            pathBuilder = t => "/" + t.FullName.Replace(".", "/");

            this.types = assemblies.SelectMany(s => s.GetTypes())
              .Where(IsRequest);
        }

        public RouteBuilder Filter(Func<Type, bool> filter)
        {
            types = types.Where(filter);
            return this;
        }

        public RouteBuilder Path(Func<Type, string> pathBuilder)
        {
            this.pathBuilder = pathBuilder;
            return this;
        }

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
        /// Gets a response type from a request type
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
