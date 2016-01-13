namespace PingPongr
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// A simple default media type handler.
    /// Uses <see cref="SimpleJson"/> internally
    /// </summary>
    public class DefaultJsonMediaHandler : IMediaTypeHandler
    {
        public bool CanHandleMediaType(string mediaType)
        {
            return mediaType == "application/json";
        }

        public static async Task<string> GetStringFromStreamAsync(Stream stream)
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public async Task<T> Read<T>(Stream inputStream, CancellationToken cancellationToken)
        {
            var json = await GetStringFromStreamAsync(inputStream);

            return SimpleJson.DeserializeObject<T>(json);
        }

        public async Task Write(object content, Stream outputStream, IRequestContext context, CancellationToken cancellationToken)
        {
            context.ResponseMediaTypes = new[] { "application/json" };

            using (StreamWriter writer = new StreamWriter(outputStream))
            {
                await writer.WriteAsync(SimpleJson.SerializeObject(content));
            }
        }
    }
}
