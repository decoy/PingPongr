namespace PingPongr.Serialization.JsonNet
{
    using Newtonsoft.Json;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using System;

    /// <summary>
    /// A simple default media type handler.
    /// Uses <see cref="Newtonsoft.Json"/> internally
    /// </summary>
    public class JsonNetMediaHandler : IMediaTypeHandler
    {
        private readonly JsonSerializer serializer;
        private static readonly Task CompletedTask = Task.FromResult(false);

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonNetMediaHandler"/> class,
        /// with the provided <paramref name="serializer"/>.
        /// </summary>
        /// <param name="serializer">Json converters used when serializing.</param>
        public JsonNetMediaHandler(JsonSerializer serializer)
        {
            if (serializer == null)
            {
                throw new ArgumentNullException("serializer");
            }
            this.serializer = serializer;
        }

        /// <summary>
        /// Creates a <see cref="JsonNetMediaHandler"/> using the <see cref="JsonSerializer.CreateDefault()"/> serializer
        /// </summary>
        /// <returns></returns>
        public static JsonNetMediaHandler CreateDefault()
        {
            return new JsonNetMediaHandler(JsonSerializer.CreateDefault());
        }

        /// <summary>
        /// Handles application/json, text/json, +json content types.
        /// </summary>
        /// <param name="contentType"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Reads the data the from the RequestBody stream.
        /// Note, runs synchronously
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task<T> Read<T>(IRequestContext context)
        {
            using (var reader = new StreamReader(context.RequestBody))
            using (var jsonReader = new JsonTextReader(reader))
            {
                return Task.FromResult(serializer.Deserialize<T>(jsonReader));
            }
        }

        /// <summary>
        /// Writes the specified content to the ResponseBody
        /// Note, runs synchronously
        /// </summary>
        /// <param name="content"></param>
        /// <param name="context"></param>
        /// <returns></returns>
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
