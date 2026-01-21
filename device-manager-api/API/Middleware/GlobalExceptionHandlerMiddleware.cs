using System.Net;
using System.Text.Json;
using device_manager_api.Application.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace device_manager_api.API.Middleware;

/// <summary>
/// Global exception handler middleware
/// </summary>
public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

        var (statusCode, title, detail) = exception switch
        {
            DbUpdateConcurrencyException => (
                HttpStatusCode.Conflict,
                "Concurrency Conflict",
                "The resource was modified by another user. Please refresh and try again."
            ),
            ConcurrencyException => (
                HttpStatusCode.Conflict,
                "Concurrency Conflict",
                exception.Message
            ),
            DeviceInUseException => (
                HttpStatusCode.Conflict,
                "Device In Use",
                exception.Message
            ),
            InvalidPatchOperationException => (
                HttpStatusCode.BadRequest,
                "Invalid Patch Operation",
                exception.Message
            ),
            DomainException => (
                HttpStatusCode.BadRequest,
                "Domain Error",
                exception.Message
            ),
            KeyNotFoundException => (
                HttpStatusCode.NotFound,
                "Resource Not Found",
                exception.Message
            ),
            UnauthorizedAccessException => (
                HttpStatusCode.Forbidden,
                "Access Forbidden",
                exception.Message
            ),
            _ => (
                HttpStatusCode.InternalServerError,
                "Internal Server Error",
                _environment.IsDevelopment() ? exception.Message : "An error occurred while processing your request."
            )
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;

        var problemDetails = new
        {
            type = $"https://httpstatuses.com/{(int)statusCode}",
            title,
            status = (int)statusCode,
            detail,
            instance = context.Request.Path.ToString(),
            traceId = context.TraceIdentifier
        };

        var json = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
