namespace PingPongr.Tests
{
    using Shouldly;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using Xunit;
    using Serialization.JsonNet;
    using Newtonsoft.Json;

    public class JsonSerializerTests
    {
        public class FakeRequest : IRequestContext, IDisposable
        {
            public bool IsHandled { get; set; }

            public string Path { get; set; }

            public Stream RequestBody { get; set; } = new MemoryStream();

            public string RequestMediaType { get; set; }

            public Stream ResponseBody { get; set; } = new MemoryStream();

            public IEnumerable<string> ResponseMediaTypes { get; set; }

            public CancellationToken CancellationToken { get; set; } = CancellationToken.None;

            public void Dispose()
            {
                RequestBody.Dispose();
                ResponseBody.Dispose();
            }
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("garbage", false)]
        [InlineData("application/json", true)]
        [InlineData("application/json; charset=UTF8", true)]
        [InlineData("aPpliCation/JSON;;;", true)]
        [InlineData("text/json", true)]
        [InlineData("application/vnd[stuff]+json", true)]
        public void ShouldHandleTypes(string header, bool isValid)
        {
            var handler = new JsonNetMediaHandler(JsonSerializer.CreateDefault());

            handler.CanHandleMediaType(header).ShouldBe(isValid);
        }

        public class DummyData
        {
            public string Test { get; set; }
            public DateTime Date { get; set; }
        }

        [Fact]
        public async void ShouldUseProvidedSerializer()
        {
            var settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
            };
            var serializer = JsonSerializer.Create(settings);
            var handler = new JsonNetMediaHandler(serializer);

            var data = new DummyData()
            {
                Test = null,
                Date = new DateTime(2000, 1, 30),
            };

            using (var req = new FakeRequest())
            {
                await handler.Write(data, req);

                req.ResponseBody.Flush();
                req.ResponseBody.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(req.ResponseBody))
                {
                    var txt = reader.ReadToEnd();
                    txt.ShouldNotContain("Test");
                    txt.ShouldContain("\"Date\":\"2000-01-30T00:00:00\"");
                }
            }
        }

        [Fact]
        public async void ShouldDeserializeData()
        {

            var serializer = JsonSerializer.Create();
            var handler = new JsonNetMediaHandler(serializer);

            using (var req = new FakeRequest())
            {
                using (var writer = new StreamWriter(req.RequestBody, System.Text.Encoding.UTF8, 1024, true))
                {
                    writer.Write("{\"Test\":\"Hello\",\"Date\":\"2000-01-30T00:00:00\"}");
                }

                req.RequestBody.Seek(0, SeekOrigin.Begin); //reset the request stream

                var results = await handler.Read<DummyData>(req);

                results.Test.ShouldBe("Hello");
                results.Date.ShouldBe(new DateTime(2000, 01, 30));
            }
        }

    }
}
