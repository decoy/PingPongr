namespace PingPongr.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Linq;
    using System.Reflection;

    [TestClass]
    public class RouteBuilderTests
    {
        public class Ping : IRouteRequest<Pong>
        {
            public string Message { get; set; }
        }

        public class Pong
        {
            public string Reply { get; set; }
        }

        public class Foo : IRouteRequest<Bar>
        {
            public string Foogle { get; set; }
        }

        public class Bar
        {
            public string Bargle { get; set; }
        }

        public class AbstractRequest<TResponse> : IRouteRequest<TResponse>
        {
            public int Id { get; set; }
        }

        public class ImplementedAbstractRequest : AbstractRequest<Bar>
        {
            public string Message { get; set; }
        }

        public interface IGenericRequest<TData, TResponse> : IRouteRequest<TResponse>
        {
            TData Data { get; set; }
        }

        public class ImplementedGenericRequest : IGenericRequest<Guid, Pong>
        {
            public Guid Data { get; set; }
        }

        public class MultipleResponseRequest : IRouteRequest<Pong>, IRouteRequest<Bar>
        {
            public string Message { get; set; }
        }

        [TestMethod]
        public void ShouldFilterRoutesByRequest()
        {
            var builder = new RouteBuilder(new[] { typeof(Ping).GetTypeInfo().Assembly });

            builder.WithFilter(t => t.FullName.Contains("RouteBuilderTests"));
            var routes = builder.GetRoutes();

            Assert.AreEqual(6, routes.Count());
            Assert.IsNotNull(routes.SingleOrDefault(r => r is Route<Ping, Pong> && r.Path.EndsWith("Ping")));
            Assert.IsNotNull(routes.SingleOrDefault(r => r is Route<Foo, Bar> && r.Path.EndsWith("Foo")));
            Assert.IsNotNull(routes.SingleOrDefault(r => r is Route<ImplementedAbstractRequest, Bar> && r.Path.EndsWith("ImplementedAbstractRequest")));
            Assert.IsNotNull(routes.SingleOrDefault(r => r is Route<ImplementedGenericRequest, Pong> && r.Path.EndsWith("ImplementedGenericRequest")));

            // the multiples will have the same path using the default path builder
            Assert.IsNotNull(routes.SingleOrDefault(r => r is Route<MultipleResponseRequest, Pong> && r.Path.EndsWith("MultipleResponseRequest")));
            Assert.IsNotNull(routes.SingleOrDefault(r => r is Route<MultipleResponseRequest, Bar> && r.Path.EndsWith("MultipleResponseRequest")));
        }

        [TestMethod]
        public void ShouldFilterRoutesByResponses()
        {
            var builder = new RouteBuilder(new[] { typeof(Ping).GetTypeInfo().Assembly });

            builder.WithFilter((treq, tresp) => tresp == typeof(Bar));

            var routes = builder.GetRoutes();

            Assert.AreEqual(3, routes.Count());
            Assert.IsNotNull(routes.SingleOrDefault(r => r is Route<Foo, Bar> && r.Path.EndsWith("Foo")));
            Assert.IsNotNull(routes.SingleOrDefault(r => r is Route<ImplementedAbstractRequest, Bar> && r.Path.EndsWith("ImplementedAbstractRequest")));
            Assert.IsNotNull(routes.SingleOrDefault(r => r is Route<MultipleResponseRequest, Bar> && r.Path.EndsWith("MultipleResponseRequest")));

        }

        [TestMethod]
        public void ShouldSetPathsForRoutesByRequest()
        {
            var builder = new RouteBuilder(new[] { typeof(Ping).GetTypeInfo().Assembly });
            builder.WithFilter(t => t.FullName.Contains("RouteBuilderTests"));

            builder.WithPathBuilder(t => "/BuilderTest/" + t.Name);

            var routes = builder.GetRoutes();

            Assert.AreEqual(6, routes.Count());

            Assert.IsNotNull(routes.SingleOrDefault(r => r.Path == "/BuilderTest/Ping"));
            Assert.IsNotNull(routes.SingleOrDefault(r => r.Path == "/BuilderTest/Foo"));
            Assert.IsNotNull(routes.SingleOrDefault(r => r.Path == "/BuilderTest/ImplementedAbstractRequest"));
            Assert.IsNotNull(routes.SingleOrDefault(r => r.Path == "/BuilderTest/ImplementedGenericRequest"));

            // by default, the multiple request will have the same default path
            // this would throw an error in the router
            Assert.AreEqual(2, routes.Count(r => r.Path == "/BuilderTest/MultipleResponseRequest"));
        }

        [TestMethod]
        public void ShouldSetPathsForRoutesByRequestAndResponse()
        {
            var builder = new RouteBuilder(new[] { typeof(Ping).GetTypeInfo().Assembly });
            builder.WithFilter(t => t.FullName.Contains("RouteBuilderTests"));

            builder.WithPathBuilder((treq, tresp) => "/BuilderTest/" + treq.Name + "/" + tresp.Name);

            var routes = builder.GetRoutes();

            Assert.AreEqual(6, routes.Count());

            Assert.IsNotNull(routes.SingleOrDefault(r => r.Path == "/BuilderTest/Ping/Pong"));
            Assert.IsNotNull(routes.SingleOrDefault(r => r.Path == "/BuilderTest/Foo/Bar"));
            Assert.IsNotNull(routes.SingleOrDefault(r => r.Path == "/BuilderTest/ImplementedAbstractRequest/Bar"));
            Assert.IsNotNull(routes.SingleOrDefault(r => r.Path == "/BuilderTest/ImplementedGenericRequest/Pong"));
            Assert.IsNotNull(routes.SingleOrDefault(r => r.Path == "/BuilderTest/MultipleResponseRequest/Pong"));
            Assert.IsNotNull(routes.SingleOrDefault(r => r.Path == "/BuilderTest/MultipleResponseRequest/Bar"));
        }
    }
}

