namespace PingPongr
{
    using System.IO;
    using System.Threading.Tasks;

    public interface IMediaTypeHandler
    {
        bool CanHandleMediaType(string mediaType);

        Task<T> Read<T>(Stream inputStream);

        Task Write(object content, Stream outputStream, IRequestContext context);
    }
}
