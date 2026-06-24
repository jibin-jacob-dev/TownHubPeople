using System.Net;
using System.Text.Json;

namespace TownHubPeople.Web.Middleware;

public class GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception occurred. TraceId: {TraceId}", context.TraceIdentifier);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var isAjax = context.Request.Headers.XRequestedWith == "XMLHttpRequest"
            || context.Request.Headers.Accept.ToString().Contains("application/json");

        if (isAjax)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = exception switch
            {
                UnauthorizedAccessException => (int)HttpStatusCode.Forbidden,
                FluentValidation.ValidationException => (int)HttpStatusCode.BadRequest,
                _ => (int)HttpStatusCode.InternalServerError
            };

            var response = new { error = "An unexpected error occurred.", traceId = context.TraceIdentifier };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        else
        {
            context.Response.Redirect($"/Home/Error?traceId={context.TraceIdentifier}");
        }
    }
}
