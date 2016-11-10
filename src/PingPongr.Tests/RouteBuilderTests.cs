namespace PingPongr.Tests
{
    using Shouldly;
    using System.Linq;
    using Xunit;
    using System.Reflection;


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

        [Fact]
        public void ShouldFilterRoutes()
        {
            var builder = new RouteBuilder(new[] { typeof(Ping).GetTypeInfo().Assembly });

            builder.Filter(t => t.FullName.Contains("RouteBuilderTests"));
            var routes = builder.GetRoutes();

            routes.Count().ShouldBe(4);
            routes.ShouldContain(r => r is Route<Ping, Pong>);
            routes.ShouldContain(r => r is Route<Ping2, Pong>);
            routes.ShouldContain(r => r is Route<PingConcrete, Pong>);
            routes.ShouldContain(r => r is Route<Ping2Concrete, Pong>);
        }

        [Fact]
        public void ShouldSetPathsForRoutes()
        {
            var builder = new RouteBuilder(new[] { typeof(Ping).GetTypeInfo().Assembly });
            builder.Filter(t => t.FullName.Contains("RouteBuilderTests"));

            builder.Path(t => "/BuilderTest/" + t.Name);
            var routes = builder.GetRoutes();

            routes.ShouldContain(r => r.Path == "/BuilderTest/Ping");
            routes.ShouldContain(r => r.Path == "/BuilderTest/Ping2");
            routes.ShouldContain(r => r.Path == "/BuilderTest/PingConcrete");
            routes.ShouldContain(r => r.Path == "/BuilderTest/Ping2Concrete");
        }
    }
}

