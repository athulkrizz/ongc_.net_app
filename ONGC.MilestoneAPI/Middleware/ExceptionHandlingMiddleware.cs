using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace ONGC.MilestoneAPI.Middleware;

/// <summary>
/// Middleware for global exception handling with ProblemDetails responses
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

        var problemDetails = new ProblemDetails();
        var statusCode = HttpStatusCode.InternalServerError;

        switch (exception)
        {
            case ArgumentException or ArgumentNullException:
                statusCode = HttpStatusCode.BadRequest;
                problemDetails.Title = "Bad Request";
                problemDetails.Detail = exception.Message;
                break;

            case InvalidOperationException:
                statusCode = HttpStatusCode.BadRequest;
                problemDetails.Title = "Invalid Operation";
                problemDetails.Detail = exception.Message;
                break;

            case UnauthorizedAccessException:
                statusCode = HttpStatusCode.Unauthorized;
                problemDetails.Title = "Unauthorized";
                problemDetails.Detail = "You are not authorized to perform this action.";
                break;

            case KeyNotFoundException:
                statusCode = HttpStatusCode.NotFound;
                problemDetails.Title = "Not Found";
                problemDetails.Detail = exception.Message;
                break;

            default:
                statusCode = HttpStatusCode.InternalServerError;
                problemDetails.Title = "Internal Server Error";
                problemDetails.Detail = "An unexpected error occurred. Please try again later.";
                break;
        }

        problemDetails.Status = (int)statusCode;
        problemDetails.Instance = context.Request.Path;
        problemDetails.Type = $"https://httpstatuses.com/{(int)statusCode}";

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/problem+json";

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(problemDetails, options);
        await context.Response.WriteAsync(json);
    }
}
