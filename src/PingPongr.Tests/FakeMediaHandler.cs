namespace PingPongr.Tests
{
    using System.Threading.Tasks;

    public class FakeMedia : IMediaTypeHandler
    {
        public bool HasRead { get; private set; }

        public bool HasWritten { get; private set; }

        public bool CanHandle(string contentType)
        {
            return true;
        }

        public Task<T> Read<T>(IRequestContext context)
        {
            HasRead = true;
            return Task.FromResult(default(T));
        }

        public Task Write<T>(IRequestContext context, T content)
        {
            HasWritten = true;
            return Task.CompletedTask;
        }
    }
}
