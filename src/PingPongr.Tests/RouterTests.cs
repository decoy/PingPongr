namespace PingPongr.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    [TestClass]
    public class RouterTests
    {
        public class Ping : IRouteRequest<Pong>
        {
            public string Message { get; set; }
        }

        public class Pong
        {
            public string Message { get; set; }
        }

        public class FakeHandler : IRouteRequestHandler<Ping, Pong>
        {
            public bool HasHandled { get; private set; }
            public Task<Pong> Handle(Ping message, CancellationToken cancellationToken)
            {
                HasHandled = true;
                return Task.FromResult(new Pong { Message = "FakePong" });
            }
        }

        public class FakeCancelMeHandler : IRouteRequestHandler<Ping, Pong>
        {
            public bool HasHandled { get; private set; }
            public Task<Pong> Handle(Ping message, CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested(); //EJECT!

                HasHandled = true;
                return Task.FromResult(new Pong { Message = "FakePong" });
            }
        }

        public class FakeRoute : IRoute
        {
            public bool HasSent { get; private set; }

            public string Path { get; set; }

            public Task Send(IRequestContext context, IEnumerable<IRouterMiddleware> middlewares)
            {
                HasSent = true;
                return Task.CompletedTask;
            }
        }

        [TestMethod]
        public async Task ShouldRoute()
        {
            var handler = new FakeHandler();
            var route = new Route<Ping, Pong>("/Ping");
            var request = new FakeRequest();
            request.AddHandlerService(handler);
            var media = new FakeMedia();

            await route.Send(request, new[] { media });

            Assert.IsTrue(media.HasRead);
            Assert.IsTrue(media.HasWritten);
            Assert.IsTrue(handler.HasHandled);
        }

        [TestMethod]
        public async Task ShouldMatchRoute()
        {
            var handler = new FakeHandler();
            var route = new FakeRoute() { Path = "/Ping" };
            var request = new FakeRequest() { Path = "/Ping" };
            var media = new FakeMedia();

            var router = new Router(new[] { route }, new[] { media });

            await router.RouteRequest(request);

            Assert.IsTrue(route.HasSent);
        }

        [TestMethod]
        public async Task ShouldNotMatchWrongRoute()
        {
            var handler = new FakeHandler();
            var route = new FakeRoute() { Path = "/Ping" };
            var request = new FakeRequest() { Path = "/NotPing" };
            var media = new FakeMedia();

            var router = new Router(new[] { route }, new[] { media });

            await router.RouteRequest(request);

            Assert.IsFalse(request.IsHandled);
            Assert.IsFalse(route.HasSent);
        }

        [TestMethod]
        public async Task ShouldCancelRequest()
        {
            var handler = new FakeCancelMeHandler();
            var route = new Route<Ping, Pong>("/Ping");
            var request = new FakeRequest();
            request.AddHandlerService(handler);
            var media = new FakeMedia();
            var cancel = new CancellationTokenSource();
            request.CancellationToken = cancel.Token;

            cancel.Cancel();
            var routeTask = route.Send(request, new[] { media });

            //standard async pattern throws on cancellation
            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() => routeTask);

            Assert.IsTrue(routeTask.IsCanceled);
            Assert.IsFalse(handler.HasHandled);
            Assert.IsFalse(media.HasWritten);
        }
    }
}
