using MongoDB.Application.Common;
using System.Net;
using System.Text.Json;

namespace MongoDB.API.Middleware;

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
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception has occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            ArgumentException => new ApiResponse<object>
            {
                Success = false,
                Message = "Invalid argument provided",
                Errors = new List<string> { exception.Message }
            },
            UnauthorizedAccessException => new ApiResponse<object>
            {
                Success = false,
                Message = "Unauthorized access",
                Errors = new List<string> { exception.Message }
            },
            KeyNotFoundException => new ApiResponse<object>
            {
                Success = false,
                Message = "Resource not found",
                Errors = new List<string> { exception.Message }
            },
            TimeoutException => new ApiResponse<object>
            {
                Success = false,
                Message = "Request timeout",
                Errors = new List<string> { exception.Message }
            },
            _ => new ApiResponse<object>
            {
                Success = false,
                Message = "An internal server error occurred",
                Errors = new List<string> { "Please try again later or contact support if the problem persists" }
            }
        };

        context.Response.StatusCode = exception switch
        {
            ArgumentException => (int)HttpStatusCode.BadRequest,
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
            KeyNotFoundException => (int)HttpStatusCode.NotFound,
            TimeoutException => (int)HttpStatusCode.RequestTimeout,
            _ => (int)HttpStatusCode.InternalServerError
        };

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}