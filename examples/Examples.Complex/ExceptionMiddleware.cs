using Microsoft.AspNetCore.Http.Features;
using PingPongr;
using PingPongr.Extensions.AspNetCore;
using System;
using System.Threading.Tasks;

namespace Examples.Complex
{
    /// <summary>
    /// An example of a PingPongr middleware handling exceptions
    /// </summary>
    public class ExceptionMiddleware : IRouterMiddleware
    {
        public async Task<TResponse> Route<TRequest, TResponse>(IRequestContext context, RequestHandlerDelegate<TRequest, TResponse> handler, RouteMiddlewareDelegate<TResponse> next)
        {
            try
            {
                // run the request and get the response
                var resp = await next();

                // We could inspect the response here in addition to or instead of checking for exceptions.
                // But in this example we'll just return it.
                return resp;
            }
            catch (UnauthorizedAccessException ex)
            {
                SetHttpContextStatus(context, 401, ex.Message);
            }
            catch (ArgumentException ex)
            {
                SetHttpContextStatus(context, 400, ex.Message);
            }

            return default(TResponse); // no data to pass on to any other middlewares
        }

        private static void SetHttpContextStatus(IRequestContext requestContext, int code, string message)
        {
            // PingPongr doesn't know anything about HTTP protocols
            // so we have to get the HTTP context from the request here
            if (requestContext is AspNetRequestContext ctx)
            {
                ctx.HttpContext.Response.StatusCode = code;
                ctx.HttpContext.Response.HttpContext
                   .Features
                   .Get<IHttpResponseFeature>()
                   .ReasonPhrase = message;
            }
        }
    }
}
