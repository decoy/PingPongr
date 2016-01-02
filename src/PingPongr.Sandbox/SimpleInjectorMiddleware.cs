namespace PingPongr.Sandbox
{
    using Microsoft.Owin;
    using Owin;
    using SimpleInjector;
    using SimpleInjector.Extensions.ExecutionContextScoping;
    using System.Runtime.Remoting.Messaging;

    /// <summary>
    /// http://simpleinjector.readthedocs.org/en/latest/owinintegration.html
    /// </summary>
    public static class SimpleInjectorMiddleware
    {
        public static void UseSimpleInjectorMiddleware(this IAppBuilder app, Container container)
        {
            app.Use(async (context, next) =>
            {
                CallContext.LogicalSetData("IOwinContext", context);
                using (container.BeginExecutionContextScope())
                {
                    await next();
                }
            });
        }
    }

    public interface IOwinContextProvider
    {
        IOwinContext CurrentContext { get; }
    }

    public class CallContextOwinContextProvider : IOwinContextProvider
    {
        public IOwinContext CurrentContext
        {
            get { return (IOwinContext)CallContext.LogicalGetData("IOwinContext"); }
        }
    }
}
