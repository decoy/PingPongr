namespace PingPongr.Mediator
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Factory method for creating a single instance of a type.
    /// Used to create handlers like <see cref="IRequestHandler{TRequest, TResponse}"/>
    /// </summary>
    /// <param name="serviceType">The type of service to be resolved/created</param>
    /// <returns>The instance of the requested type</returns>
    public delegate object InstanceFactory(Type serviceType);

    /// <summary>
    /// Factory method for creating a multiple instance of a types
    /// Used to create multiple handlers like <see cref="IRequestHandler{TRequest, TResponse}"/>
    /// </summary>
    /// <param name="serviceType">The type of service to be resolved/created</param>
    /// <returns>An enumerable of instances of the requested type</returns>
    public delegate IEnumerable<object> MultiInstanceFactory(Type serviceType);

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

        /// <summary>
        /// Resolves multiple instances of the specified type
        /// </summary>
        /// <typeparam name="T">he type to be resolved</typeparam>
        /// <param name="factory">The factory to be used</param>
        /// <returns>Enumerable of the resolved instances</returns>
        public static IEnumerable<T> Resolve<T>(this MultiInstanceFactory factory)
        {
            return (IEnumerable<T>)factory(typeof(T));
        }
    }
}
