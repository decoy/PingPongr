namespace PingPongr
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Factory method for creating a single instance of a type.
    /// Used to create handlers like <see cref="IRouteRequestHandler{TRequest, TResponse}"/>
    /// </summary>
    /// <param name="serviceType">The type of service to be resolved/created</param>
    /// <returns>The instance of the requested type</returns>
    public delegate object InstanceFactory(Type serviceType);

    /// <summary>
    /// Quick wrappers for requesting types via generics
    /// </summary>
    public static class FactoryExtensions
    {
        /// <summary>
        /// Resolves an instance of the specified type
        /// </summary>
        /// <typeparam name="T">The type to be resolved</typeparam>
        /// <param name="factory">The factory to be used</param>
        /// <returns>A resolved instance of the type</returns>
        public static T Resolve<T>(this InstanceFactory factory)
        {
            return (T)factory(typeof(T));
        }
    }
}
