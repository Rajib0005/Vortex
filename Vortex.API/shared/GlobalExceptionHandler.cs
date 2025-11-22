using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Vortex.Domain.Dto;
using Vortex.Infrastructure.CustomException;

namespace Vortex.API;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "An unexpected error occurred: {Message}", exception.Message);

        // Determine Status Code and Message based on Exception Type
        (var statusCode, string message) = exception switch
        {
            // Custom Exceptions
            BadRequestException => (StatusCodes.Status400BadRequest, "Bad Request"),
            NotFoundException => (StatusCodes.Status404NotFound, "Resource Not Found"),
            UnauthorizedException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            ForbiddenException => (StatusCodes.Status403Forbidden, "Forbidden"),
            ConflictException => (StatusCodes.Status409Conflict, "Conflict"),
            
            // Built-in .NET Exceptions (Fallbacks)
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Not Found"),
            ArgumentException => (StatusCodes.Status400BadRequest, "Invalid Argument"),
            
            //Default
            _ => (StatusCodes.Status500InternalServerError, "An internal server error occurred.")
        };

        httpContext.Response.StatusCode = statusCode;

        // Create our standard response model
        var response = BaseResponse<object>.FailureResponse(message, new[] { exception.Message });
        var serializedJson = JsonSerializer.Serialize(response);        // Write JSON directly to response
        await httpContext.Response.WriteAsync(serializedJson, cancellationToken);

        return true;
    }
}