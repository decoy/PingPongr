namespace PingPongr
{
    using System.Collections.Generic;
    using System.IO;

    public interface IRequestContext
    {
        string Path { get; }

        bool IsHandled { get; set; }

        Stream RequestBody { get; }
        string RequestMediaType { get; }

        Stream ResponseBody { get; }
        IEnumerable<string> ResponseMediaTypes { get; set; }

    }
}