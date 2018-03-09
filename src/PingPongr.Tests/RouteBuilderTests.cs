namespace PingPongr.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    [TestClass]
    public class RouteBuilderTests
    {
        public class Ping
        {
            public string Message { get; set; }
        }

        public class Pong
        {
            public string Reply { get; set; }
        }

        // normal implementation
        public class NormalHandler : IRouteHandler<Ping, Pong>
        {
            public Task<Pong> Handle(Ping request, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }

        // abstract implementation
        public abstract class AbstractHandler : IRouteHandler<Ping, Pong>
        {
            public Task<Pong> Handle(Ping request, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }

        // implements abstract
        public class ImplementedAbstractHandler : AbstractHandler { }

        // generic implementation
        public class GenericHandler<TReq, TResp> : IRouteHandler<TReq, TResp>
        {
            public Task<TResp> Handle(TReq request, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }

        // implements generic
        public class ImplementedGenericHandler : GenericHandler<Ping, Pong> { }

        // implements two
        public class DualHandler : IRouteHandler<Ping, Pong>, IRouteHandler<Pong, Ping>
        {
            public Task<Pong> Handle(Ping request, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }

            public Task<Ping> Handle(Pong request, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }

        // implements abstract and another because why not
        public class ComplicatedHandler : AbstractHandler, IRouteHandler<Pong, Ping>
        {
            public Task<Ping> Handle(Pong request, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        public void ShouldFilterRoutes()
        {
            var builder = new RouteBuilder(new[] { typeof(Ping).GetTypeInfo().Assembly });

            builder.Filter(t => t.FullName.Contains("RouteBuilderTests"));
            var routes = builder.GetRoutes();

            Assert.AreEqual(7, routes.Count());
            Assert.IsNotNull(routes.SingleOrDefault(r => r is Route<Ping, Pong> && r.Path.EndsWith("NormalHandler")));
            Assert.IsNotNull(routes.SingleOrDefault(r => r is Route<Ping, Pong> && r.Path.EndsWith("ImplementedAbstractHandler")));
            Assert.IsNotNull(routes.SingleOrDefault(r => r is Route<Ping, Pong> && r.Path.EndsWith("ImplementedGenericHandler")));
            Assert.IsNotNull(routes.SingleOrDefault(r => r is Route<Ping, Pong> && r.Path.EndsWith("DualHandler")));
            Assert.IsNotNull(routes.SingleOrDefault(r => r is Route<Pong, Ping> && r.Path.EndsWith("DualHandler")));
            Assert.IsNotNull(routes.SingleOrDefault(r => r is Route<Ping, Pong> && r.Path.EndsWith("ComplicatedHandler")));
            Assert.IsNotNull(routes.SingleOrDefault(r => r is Route<Pong, Ping> && r.Path.EndsWith("ComplicatedHandler")));
        }

        [TestMethod]
        public void ShouldSetPathsForRoutes()
        {
            var builder = new RouteBuilder(new[] { typeof(Ping).GetTypeInfo().Assembly });
            builder.Filter(t => t.FullName.Contains("RouteBuilderTests"));

            builder.SetPathBuilder((timpl, treq, tresp) => "/BuilderTest/" + timpl.Name + "/" + treq.Name);
            var routes = builder.GetRoutes();

            Assert.AreEqual(7, routes.Count());

            Assert.IsNotNull(routes.SingleOrDefault(r => r.Path == "/BuilderTest/NormalHandler/Ping"));
            Assert.IsNotNull(routes.SingleOrDefault(r => r.Path == "/BuilderTest/ImplementedAbstractHandler/Ping"));
            Assert.IsNotNull(routes.SingleOrDefault(r => r.Path == "/BuilderTest/ImplementedGenericHandler/Ping"));
            Assert.IsNotNull(routes.SingleOrDefault(r => r.Path == "/BuilderTest/DualHandler/Ping"));
            Assert.IsNotNull(routes.SingleOrDefault(r => r.Path == "/BuilderTest/DualHandler/Pong"));
            Assert.IsNotNull(routes.SingleOrDefault(r => r.Path == "/BuilderTest/ComplicatedHandler/Ping"));
            Assert.IsNotNull(routes.SingleOrDefault(r => r.Path == "/BuilderTest/ComplicatedHandler/Pong"));
        }
    }
}

