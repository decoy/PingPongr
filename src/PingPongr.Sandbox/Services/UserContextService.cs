namespace PingPongr.Sandbox.Services
{
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// abstraction for the user context
    /// </summary>
    public interface IUserContext
    {
        string Name { get; }
        string RequestPath { get; }
    }

    /// <summary>
    /// An accessor class to working with the AspNet HttpContexts
    /// https://simpleinjector.org/blog/2016/07/working-around-the-asp-net-core-di-abstraction/
    /// </summary>
    public sealed class AspNetUserContext : IUserContext
    {
        private readonly IHttpContextAccessor accessor;
        public AspNetUserContext(IHttpContextAccessor a) { accessor = a; }
        public string Name => accessor.HttpContext.User.Identity.Name;
        public string RequestPath => accessor.HttpContext.Request.Path;
    }

}
