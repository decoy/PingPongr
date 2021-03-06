﻿namespace PingPongr.Serialization.JsonNet
{
    using Newtonsoft.Json;
    using System;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Default implementation of a Json.Net media handler.
    /// </summary>
    public class JsonNetMediaHandler : IMediaTypeHandler
    {
        private JsonSerializer serializer;

        /// <summary>
        /// Creates a media handler using the <see cref="JsonSerializer"/>
        /// </summary>
        /// <param name="serializer"></param>
        public JsonNetMediaHandler(JsonSerializer serializer)
        {
            this.serializer = serializer;
        }

        /// <summary>
        /// Creates a default serializer to use for media handling
        /// </summary>
        public JsonNetMediaHandler()
        {
            serializer = JsonSerializer.CreateDefault();
        }

        /// <summary>
        /// Checks if the content type (media type) is json.
        /// </summary>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public bool CanHandle(string contentType)
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

        /// <summary>
        /// Reads the json data from the context and deserializes it appropriately
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task<T> Read<T>(IRequestContext context)
        {
            using (StreamReader reader = new StreamReader(context.RequestBody, System.Text.Encoding.UTF8, true, 1024, true))
            using (JsonTextReader jsonReader = new JsonTextReader(reader))
            {
                return Task.FromResult(serializer.Deserialize<T>(jsonReader));
            }
        }

        /// <summary>
        /// Writes the json data to the context
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public Task Write<T>(IRequestContext context, T content)
        {
            context.ResponseContentType = "application/json";

            using (StreamWriter writer = new StreamWriter(context.ResponseBody, System.Text.Encoding.UTF8, 1024, true))
            using (JsonTextWriter jsonWriter = new JsonTextWriter(writer))
            {
                serializer.Serialize(jsonWriter, content);
                jsonWriter.Flush();
                return Task.CompletedTask;
            }
        }
    }
}
