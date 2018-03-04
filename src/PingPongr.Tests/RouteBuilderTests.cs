namespace PingPongr.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Linq;
    using System.Reflection;

    [TestClass]
    public class RouteBuilderTests
    {
        public class Ping : IRouteRequest<Pong>
        {
            public string Message { get; set; }
        }

        public class Ping2 : IRouteRequest<Pong>
        {
            public string Message { get; set; }
        }

        public abstract class PingAbs : IRouteRequest<Pong>
        {
            public string Message { get; set; }
        }

        public class PingConcrete : PingAbs { }

        public class Ping2Concrete : PingAbs { }

        public class Pong
        {
            public string Message { get; set; }
        }

        [TestMethod]
        public void ShouldFilterRoutes()
        {
            var builder = new RouteBuilder(new[] { typeof(Ping).GetTypeInfo().Assembly });

            builder.Filter(t => t.FullName.Contains("RouteBuilderTests"));
            var routes = builder.GetRoutes();

            Assert.AreEqual(4, routes.Count());
            Assert.IsNotNull(routes.SingleOrDefault(r => r is Route<Ping, Pong>));
            Assert.IsNotNull(routes.SingleOrDefault(r => r is Route<Ping2, Pong>));
            Assert.IsNotNull(routes.SingleOrDefault(r => r is Route<PingConcrete, Pong>));
            Assert.IsNotNull(routes.SingleOrDefault(r => r is Route<Ping2Concrete, Pong>));
        }

        [TestMethod]
        public void ShouldSetPathsForRoutes()
        {
            var builder = new RouteBuilder(new[] { typeof(Ping).GetTypeInfo().Assembly });
            builder.Filter(t => t.FullName.Contains("RouteBuilderTests"));

            builder.Path(t => "/BuilderTest/" + t.Name);
            var routes = builder.GetRoutes();

            Assert.AreEqual(4, routes.Count());

            Assert.IsNotNull(routes.SingleOrDefault(r => r.Path == "/BuilderTest/Ping"));
            Assert.IsNotNull(routes.SingleOrDefault(r => r.Path == "/BuilderTest/Ping2"));
            Assert.IsNotNull(routes.SingleOrDefault(r => r.Path == "/BuilderTest/PingConcrete"));
            Assert.IsNotNull(routes.SingleOrDefault(r => r.Path == "/BuilderTest/Ping2Concrete"));
        }
    }
}

