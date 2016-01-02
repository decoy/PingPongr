namespace PingPongr.Tests
{
    using Mediator;
    using Shouldly;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    public class MediatorTests
    {
        public class Ping : IRequest<Pong>
        {
            public string Message { get; set; }
        }

        public class Pong
        {
            public string Message { get; set; }
        }

        public class PingHandler : IRequestHandler<Ping, Pong>
        {
            public Pong Handle(Ping message)
            {
                return new Pong { Message = message.Message + " Pong" };
            }
        }

        public class PingAlsoHandler : IRequestHandler<Ping, Pong>
        {
            public Pong Handle(Ping message)
            {
                return new Pong { Message = message.Message + " PongAgain" };
            }
        }

        public class PingHandlerAsync : IRequestAsyncHandler<Ping, Pong>
        {
            public Task<Pong> Handle(Ping message)
            {
                return Task.FromResult(new Pong { Message = message.Message + " PongAsync" });
            }
        }


        public class PingAlsoHandlerAsync : IRequestAsyncHandler<Ping, Pong>
        {
            public Task<Pong> Handle(Ping message)
            {
                return Task.FromResult(new Pong { Message = message.Message + " PongAsyncAgain" });
            }
        }

        public class PingHandlerAsyncCancel : IRequestCancellableAsyncHandler<Ping, Pong>
        {
            public Task<Pong> Handle(Ping message, CancellationToken cancellationToken)
            {
                return Task.FromResult(new Pong { Message = message.Message + " PongAsyncCancel" });
            }
        }

        public class PingAlsoHandlerAsyncCancel : IRequestCancellableAsyncHandler<Ping, Pong>
        {
            public Task<Pong> Handle(Ping message, CancellationToken cancellationToken)
            {
                return Task.FromResult(new Pong { Message = message.Message + " PongAsyncCancelAgain" });
            }
        }


        [Fact]
        public void ShouldSendMessage()
        {
            IMediator med = new Mediator(t => new PingHandler(), null);

            var result = med.Send(new Ping() { Message = "Hi" });

            result.Message.ShouldBe("Hi Pong");
        }


        [Fact]
        public async void ShouldSendMessageAsync()
        {
            IMediator med = new Mediator(t => new PingHandlerAsync(), null);

            var result = await med.SendAsync(new Ping() { Message = "Hi" });

            result.Message.ShouldBe("Hi PongAsync");
        }

        [Fact]
        public async void ShouldSendMessageAsyncCancellable()
        {
            IMediator med = new Mediator(t => new PingHandlerAsyncCancel(), null);

            var result = await med.SendAsync(new Ping() { Message = "Hi" }, new CancellationToken());

            result.Message.ShouldBe("Hi PongAsyncCancel");
        }


        [Fact]
        public void ShouldPublishMessage()
        {
            IMediator med = new Mediator(null, t => new IRequestHandler<Ping, Pong>[] { new PingHandler(), new PingAlsoHandler() });

            var results = med.Publish(new Ping() { Message = "Hi" });

            results.Count().ShouldBe(2);
            results.ShouldContain(p => p.Message == "Hi Pong");
            results.ShouldContain(p => p.Message == "Hi PongAgain");
        }


        [Fact]
        public async void ShouldPublishMessageAsync()
        {
            IMediator med = new Mediator(null, t => new IRequestAsyncHandler<Ping, Pong>[] { new PingHandlerAsync(), new PingAlsoHandlerAsync() });

            var results = await Task.WhenAll(med.PublishAsync(new Ping() { Message = "Hi" }));

            results.Count().ShouldBe(2);
            results.ShouldContain(p => p.Message == "Hi PongAsync");
            results.ShouldContain(p => p.Message == "Hi PongAsyncAgain");
        }

        [Fact]
        public async void ShouldPublishMessageAsyncCancellable()
        {
            IMediator med = new Mediator(null, t => new IRequestCancellableAsyncHandler<Ping, Pong>[] { new PingHandlerAsyncCancel(), new PingAlsoHandlerAsyncCancel() });

            var results = await Task.WhenAll(med.PublishAsync(new Ping() { Message = "Hi" }, new CancellationToken()));

            results.Count().ShouldBe(2);
            results.ShouldContain(p => p.Message == "Hi PongAsyncCancel");
            results.ShouldContain(p => p.Message == "Hi PongAsyncCancelAgain");
        }

    }
}
