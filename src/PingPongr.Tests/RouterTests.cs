namespace PingPongr.Tests
{
    using Shouldly;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

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

        public class FakeMedia : IMediaTypeHandler
        {
            public bool HasRead { get; private set; }

            public bool HasWritten { get; private set; }

            public bool CanHandleMediaType(string mediaType)
            {
                return true;
            }

            public Task<T> Read<T>(Stream inputStream, CancellationToken cancellationToken)
            {
                HasRead = true;
                return Task.FromResult(default(T));
            }

            public Task Write(object content, Stream outputStream, IRequestContext context, CancellationToken cancellationToken)
            {
                HasWritten = true;
                return Task.FromResult(0);
            }
        }

        public class FakeRequest : IRequestContext
        {
            public bool IsHandled { get; set; }

            public string Path { get; set; }

            public Stream RequestBody { get; set; }

            public string RequestMediaType { get; set; }

            public Stream ResponseBody { get; set; }

            public IEnumerable<string> ResponseMediaTypes { get; set; }

            public CancellationToken CancellationToken { get; set; }
        }

        public class FakeRoute : IRoute
        {
            public bool HasSent { get; private set; }
            public string Path { get; set; }

            public Task Send(IMediaTypeHandler mediaHandler, InstanceFactory factory, IRequestContext context)
            {
                HasSent = true;
                return Task.FromResult(0);
            }
        }

        [Fact]
        public async void ShouldRoute()
        {
            var handler = new FakeHandler();
            var route = new Route<Ping, Pong>();
            var request = new FakeRequest();
            var media = new FakeMedia();

            await route.Send(media, t => handler, request);

            media.HasRead.ShouldBeTrue();
            media.HasWritten.ShouldBeTrue();
            handler.HasHandled.ShouldBeTrue();
        }

        [Fact]
        public async void ShouldMatchRoute()
        {
            var handler = new FakeHandler();
            var route = new FakeRoute() { Path = "/Ping" };
            var request = new FakeRequest() { Path = "/Ping" };
            var media = new FakeMedia();

            var router = new Router(new[] { route }, new[] { media }, t => handler);

            await router.RouteRequest(request);

            request.IsHandled.ShouldBeTrue();
            route.HasSent.ShouldBeTrue();
        }

        [Fact]
        public async void ShouldNotMatchWrongRoute()
        {
            var handler = new FakeHandler();
            var route = new FakeRoute() { Path = "/Ping" };
            var request = new FakeRequest() { Path = "/NotPing" };
            var media = new FakeMedia();

            var router = new Router(new[] { route }, new[] { media }, t => handler);

            await router.RouteRequest(request);

            request.IsHandled.ShouldBeFalse();
            route.HasSent.ShouldBeFalse();
        }
    }
}
