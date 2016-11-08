namespace PingPongr.Serialization.JsonNet
{
    using Newtonsoft.Json;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// A simple default media type handler.
    /// Uses <see cref="SimpleJson"/> internally
    /// </summary>
    public class JsonNetMedialHandler : IMediaTypeHandler
    {
        private readonly JsonSerializer serializer;
        private static readonly Task CompletedTask = Task.FromResult(false);

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonNetSerializer"/> class.
        /// </summary>
        public JsonNetMedialHandler()
        {
            this.serializer = JsonSerializer.CreateDefault();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonNetSerializer"/> class,
        /// with the provided <paramref name="serializer"/>.
        /// </summary>
        /// <param name="serializer">Json converters used when serializing.</param>
        public JsonNetMedialHandler(JsonSerializer serializer)
        {
            this.serializer = serializer;
        }

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
            return JsonConvert.DeserializeObject<T>(json);
        }

        public Task Write(object content, Stream outputStream, IRequestContext context, CancellationToken cancellationToken)
        {
            context.ResponseMediaTypes = new[] { "application/json" };

            using (var writer = new JsonTextWriter(new StreamWriter(outputStream)))
            {
                serializer.Serialize(writer, content);
                return CompletedTask;
            }
        }
    }
}
