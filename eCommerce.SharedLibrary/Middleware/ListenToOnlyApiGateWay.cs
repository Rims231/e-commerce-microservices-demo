using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.SharedLibrary.Middleware
{
    public class ListenToOnlyApiGateway(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            // Extract specific header from the request
            var signedHeader = context.Request.Headers["Api-Gateway"];

            // NULL means the request is not coming from the API Gateway
            if (signedHeader.FirstOrDefault() is null)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;

                await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails
                {
                    Status = (int)HttpStatusCode.ServiceUnavailable,
                    Title = "Service Unavailable",
                    Detail = "Sorry, service is unavailable. Please contact the administrator."
                }), CancellationToken.None);

                return;
            }

            await next(context);
        }
    }
}