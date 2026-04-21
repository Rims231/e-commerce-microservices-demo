using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using eCommerce.SharedLibrary.Logs;

namespace eCommerce.SharedLibrary.Middleware
{
    public class GlobalException(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            // Default values
            string message = "Sorry, internal server error occurred. Kindly try again later.";
            int statusCode = (int)HttpStatusCode.InternalServerError;
            string title = "Error";

            try
            {
                await next(context);

                // Handle 429 Too Many Requests
                if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
                {
                    message = "Too many requests made. Kindly try again later.";
                    statusCode = StatusCodes.Status429TooManyRequests;
                    title = "Warning";
                    await ModifyHeader(context, title, message, statusCode);
                }

                // Handle 401 Unauthorized
                if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                {
                    message = "You are not authorized to access this resource.";
                    statusCode = StatusCodes.Status401Unauthorized;
                    title = "Alert";
                    await ModifyHeader(context, title, message, statusCode);
                }

                // Handle 403 Forbidden
                if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                {
                    message = "You are not allowed to access this resource.";
                    statusCode = StatusCodes.Status403Forbidden;
                    title = "Out of Access";
                    await ModifyHeader(context, title, message, statusCode);
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                LogExceptions.LogException(ex);

                // Handle timeout
                if (ex is TaskCanceledException || ex is TimeoutException)
                {
                    message = "Request timed out. Kindly try again later.";
                    statusCode = StatusCodes.Status408RequestTimeout;
                    title = "Timeout";
                }

                await ModifyHeader(context, title, message, statusCode);
            }
        }

        private static async Task ModifyHeader(HttpContext context, string title, string message, int statusCode)
        {
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails
            {
                Detail = message,
                Status = statusCode,
                Title = title
            }), CancellationToken.None);
        }
    }
}