namespace PingPongr.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;

    public class FakeRequest : IRequestContext, IDisposable
    {
        public Dictionary<Type, object> Services { get; set; } = new Dictionary<Type, object>();

        public bool IsHandled { get; set; }

        public string Path { get; set; }

        public Stream RequestBody { get; set; } = new MemoryStream();

        public string RequestContentType { get; set; }

        public Stream ResponseBody { get; set; } = new MemoryStream();

        public string ResponseContentType { get; set; }

        public CancellationToken CancellationToken { get; set; } = CancellationToken.None;

        public T GetService<T>()
        {
            return (T)Services[typeof(T)];
        }

        public void Dispose()
        {
            RequestBody.Dispose();
            ResponseBody.Dispose();
        }
    }
}
