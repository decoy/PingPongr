namespace PingPongr
{
    using System.IO;
    using System.Threading.Tasks;

    public class DefaultJsonMediaHandler : IMediaTypeHandler
    {
        public bool CanHandleMediaType(string mediaType)
        {
            return mediaType == "application/json";
        }

        public static Task<string> GetStringFromStreamAsync(Stream stream)
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEndAsync();
            }
        }

        public async Task<T> Read<T>(Stream inputStream)
        {
            var json = await GetStringFromStreamAsync(inputStream);

            return SimpleJson.DeserializeObject<T>(json);
        }

        public Task Write(object content, Stream outputStream, IRequestContext context)
        {
            context.ResponseMediaTypes = new[] { "application/json" };

            using (StreamWriter writer = new StreamWriter(outputStream))
            {
                return writer.WriteAsync(SimpleJson.SerializeObject(content));
            }
        }
    }
}
