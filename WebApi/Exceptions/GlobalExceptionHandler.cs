using Microsoft.AspNetCore.Diagnostics;
using ProductCatalogAPI.DTOs;
using System.Net;
using System.Text.Json;

namespace ProductCatalogAPI.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Log the exception
        _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

        // Determine status code based on exception type
        var statusCode = exception switch
        {
            ArgumentException => HttpStatusCode.BadRequest,
            KeyNotFoundException => HttpStatusCode.NotFound,
            UnauthorizedAccessException => HttpStatusCode.Unauthorized,
            _ => HttpStatusCode.InternalServerError
        };

        // Build clean error response
        var response = ApiResponseDto<object>.ErrorResult(
            error: exception.Message,
            message: GetMessageFromStatusCode(statusCode)
        );

        httpContext.Response.StatusCode = (int)statusCode;
        httpContext.Response.ContentType = "application/json";

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await httpContext.Response.WriteAsync(json, cancellationToken);

        return true;
    }

    private string GetMessageFromStatusCode(HttpStatusCode statusCode)
    {
        return statusCode switch
        {
            HttpStatusCode.BadRequest => "Bad request",
            HttpStatusCode.NotFound => "Resource not found",
            HttpStatusCode.Unauthorized => "Unauthorized access",
            _ => "An unexpected error occurred"
        };
    }
}