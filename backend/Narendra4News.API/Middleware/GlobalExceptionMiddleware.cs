using System.Net;
using System.Text.Json;
using Narendra4News.Application.Common;

namespace Narendra4News.API.Middleware;

// Ensures every unhandled error is still returned in the app-wide
// { success, message, data } envelope, and logs it server-side (spec section 23/35).
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (KeyNotFoundException ex)
        {
            await WriteAsync(context, HttpStatusCode.NotFound, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            await WriteAsync(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            await WriteAsync(context, HttpStatusCode.Forbidden, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception processing {Method} {Path}", context.Request.Method, context.Request.Path);
            await WriteAsync(context, HttpStatusCode.InternalServerError, "An unexpected error occurred.");
        }
    }

    private static async Task WriteAsync(HttpContext context, HttpStatusCode statusCode, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        var response = ApiResponse<object>.Fail(message);
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions(JsonSerializerDefaults.Web)));
    }
}
