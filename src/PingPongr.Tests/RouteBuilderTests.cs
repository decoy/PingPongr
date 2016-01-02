namespace PingPongr.Tests
{
    using Mediator;
    using Shouldly;
    using System.Linq;
    using Xunit;

    public class RouteBuilderTests
    {
        public class Ping : IRequest<Pong>
        {
            public string Message { get; set; }
        }

        public class Ping2 : IRequest<Pong>
        {
            public string Message { get; set; }
        }

        public abstract class PingAbs : IRequest<Pong>
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
            var builder = new RouteBuilder(new[] { typeof(RouteBuilderTests).Assembly });

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
            var builder = new RouteBuilder(new[] { typeof(RouteBuilderTests).Assembly });
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

