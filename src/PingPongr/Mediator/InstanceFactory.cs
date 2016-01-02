namespace PingPongr.Mediator
{
    using System;
    using System.Collections.Generic;

    public delegate object InstanceFactory(Type serviceType);

    public delegate IEnumerable<object> MultiInstanceFactory(Type serviceType);

    public static class FactoryExtensions
    {
        public static T Create<T>(this InstanceFactory factory)
        {
            return (T)factory(typeof(T));
        }

        public static IEnumerable<T> Create<T>(this MultiInstanceFactory factory)
        {
            return (IEnumerable<T>)factory(typeof(T));
        }
    }
}
