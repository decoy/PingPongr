namespace PingPongr.Tests
{
    using OwinSupport;
    using Shouldly;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using Xunit;

    public class OwinTests
    {

        private Dictionary<string, object> GetOwinEnvironment()
        {
            //these are all set by default from owin contexts
            var env = new Dictionary<string, object>();
            env.Add("owin.ResponseStatusCode", 200);
            env.Add("owin.RequestPath", "/Ping");
            env.Add("owin.RequestMethod", "POST");
            env.Add("owin.ResponseHeaders", new Dictionary<string, string[]>());
            env.Add("owin.RequestHeaders", new Dictionary<string, string[]>());
            env.Add("owin.ResponseBody", new MemoryStream());
            env.Add("owin.RequestBody", new MemoryStream());
            env.Add("owin.CallCancelled", CancellationToken.None);

            return env;
        }

        [Fact]
        public void ShouldRequireRequestBody()
        {
            var env = GetOwinEnvironment();
            env["owin.RequestBody"] = null;

            var ex = Should.Throw<ArgumentNullException>(() => new OwinContext(env, null));
            ex.ParamName.ShouldBe("owin.RequestBody");
        }

        [Fact]
        public void ShouldRequireResponseBody()
        {
            var env = GetOwinEnvironment();
            env["owin.ResponseBody"] = null;

            var ex = Should.Throw<ArgumentNullException>(() => new OwinContext(env, null));
            ex.ParamName.ShouldBe("owin.ResponseBody");
        }

        [Fact]
        public void ShouldRequireRequestPath()
        {
            var env = GetOwinEnvironment();
            env["owin.RequestPath"] = null;

            var ex = Should.Throw<ArgumentNullException>(() => new OwinContext(env, null));
            ex.ParamName.ShouldBe("owin.RequestPath");
        }

        [Fact]
        public void ShouldTrimPrefixFromRequestPath()
        {
            var env = GetOwinEnvironment();
            env["owin.RequestPath"] = "/api/Ping";

            var ctx = new OwinContext(env, "/api");

            ctx.Path.ShouldBe("/Ping");
        }

        [Fact]
        public void ShouldSetResponseTypes()
        {
            var env = GetOwinEnvironment();
            var ctx = new OwinContext(env, null);

            ctx.ResponseMediaTypes = new[] { "application/json" };
            ctx.ResponseMediaTypes.ShouldContain("application/json");
        }

        [Fact]
        public void ShouldGetRequestMediaType()
        {
            var env = GetOwinEnvironment();
            ((IDictionary<string, string[]>)env["owin.RequestHeaders"])["Content-Type"] = new[] { "application/json" };
            var ctx = new OwinContext(env, null);

            ctx.RequestMediaType.ShouldBe("application/json");
        }

        [Fact]
        public void ShouldCreateFromEnvironent()
        {
            //sanity check that everything creates
            var env = GetOwinEnvironment();
            var ctx = new OwinContext(env, null);

            ctx.CancellationToken.ShouldBe(CancellationToken.None);
            ctx.Method.ShouldBe("POST");
            ctx.Path.ShouldBe("/Ping");
            ctx.RequestPath.ShouldBe("/Ping");
            ctx.ResponseHeaders.ShouldNotBeNull();
            ctx.RequestHeaders.ShouldNotBeNull();
            ctx.ResponseBody.ShouldNotBeNull();
            ctx.RequestBody.ShouldNotBeNull();
        }

        [Fact]
        public void ShouldNotBeHandledWith404()
        {
            var env = GetOwinEnvironment();
            env["owin.ResponseStatusCode"] = 404;

            var ctx = new OwinContext(env, null);

            ctx.IsHandled.ShouldBeFalse();
        }

        [Fact]
        public void ShouldBeHandledWith200()
        {
            var env = GetOwinEnvironment();
            env["owin.ResponseStatusCode"] = 200;

            var ctx = new OwinContext(env, null);

            ctx.IsHandled.ShouldBeTrue();
        }

    }
}
