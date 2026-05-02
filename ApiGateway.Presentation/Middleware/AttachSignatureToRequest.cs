using Microsoft.AspNetCore.Http;

namespace ApiGateway.Presentation.Middleware
{
    public class AttachSignatureToRequest(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            // Attach the Api-Gateway header to every request
            // before forwarding to downstream services
            context.Request.Headers["Api-Gateway"] = "approved";

            await next(context);
        }
    }
}