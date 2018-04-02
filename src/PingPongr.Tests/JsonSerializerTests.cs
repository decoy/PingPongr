namespace PingPongr.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json;
    using PingPongr.Serialization.JsonNet;
    using System;
    using System.IO;
    using System.Threading.Tasks;

    [TestClass]
    public class JsonSerializerTests
    {
        private class DummyData
        {
            public string Test { get; set; }
            public DateTime Date { get; set; }
        }

        [DataTestMethod]
        [DataRow(null, false)]
        [DataRow("garbage", false)]
        [DataRow("application/json", true)]
        [DataRow("application/json; charset=UTF8", true)]
        [DataRow("aPpliCation/JSON;;;", true)]
        [DataRow("text/json", true)]
        [DataRow("application/vnd[stuff]+json", true)]
        public void ShouldHandleTypes(string header, bool isValid)
        {
            var handler = new JsonNetMediaHandler();

            Assert.AreEqual(handler.CanHandle(header), isValid);
        }

        [TestMethod]
        public async Task ShouldUseProvidedSerializer()
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
                await handler.Write(req, data);

                req.ResponseBody.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(req.ResponseBody))
                {
                    var txt = reader.ReadToEnd();
                    Assert.IsFalse(txt.Contains("Test"));
                    Assert.IsTrue(txt.Contains("\"Date\":\"2000-01-30T00:00:00\""));
                }
            }
        }

        [TestMethod]
        public async Task ShouldDeserializeData()
        {
            var handler = new JsonNetMediaHandler(JsonSerializer.CreateDefault());

            using (var req = new FakeRequest())
            {
                using (var writer = new StreamWriter(req.RequestBody, System.Text.Encoding.UTF8, 1024, true))
                {
                    writer.Write("{\"Test\":\"Hello\",\"Date\":\"2000-01-30T00:00:00\"}");
                }

                req.RequestBody.Seek(0, SeekOrigin.Begin); //reset the request stream

                var results = await handler.Read<DummyData>(req);

                Assert.AreEqual("Hello", results.Test);
                Assert.AreEqual(new DateTime(2000, 01, 30), results.Date);
            }
        }
    }
}
