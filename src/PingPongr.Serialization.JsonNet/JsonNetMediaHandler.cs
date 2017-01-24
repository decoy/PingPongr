namespace PingPongr.Serialization.JsonNet
{
    using Newtonsoft.Json;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using System;

    /// <summary>
    /// A simple default media type handler.
    /// Uses <see cref="SimpleJson"/> internally
    /// </summary>
    public class JsonNetMediaHandler : IMediaTypeHandler
    {
        private readonly JsonSerializer serializer;
        private static readonly Task CompletedTask = Task.FromResult(false);

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonNetSerializer"/> class,
        /// with the provided <paramref name="serializer"/>.
        /// </summary>
        /// <param name="serializer">Json converters used when serializing.</param>
        public JsonNetMediaHandler(JsonSerializer serializer)
        {
            this.serializer = serializer;
        }

        public bool CanHandleMediaType(string contentType)
        {
            if (string.IsNullOrEmpty(contentType))
            {
                return false;
            }

            //grab the first part of the media type (ignoring any charset information)
            var mimeType = contentType.Split(';')[0];

            return mimeType.Equals("application/json", StringComparison.OrdinalIgnoreCase)
                   || mimeType.Equals("text/json", StringComparison.OrdinalIgnoreCase)
                   || mimeType.EndsWith("+json", StringComparison.OrdinalIgnoreCase);
        }

        private static async Task<string> GetStringFromStreamAsync(Stream stream)
        {
            using (StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8, true, 1024, true))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public async Task<T> Read<T>(IRequestContext context)
        {
            var json = await GetStringFromStreamAsync(context.RequestBody);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public Task Write(object content, IRequestContext context)
        {
            context.ResponseMediaTypes = new[] { "application/json" };

            using (var writer = new JsonTextWriter(new StreamWriter(context.ResponseBody, System.Text.Encoding.UTF8, 1024, true)))
            {
                serializer.Serialize(writer, content);
                return CompletedTask;
            }
        }
    }
}
