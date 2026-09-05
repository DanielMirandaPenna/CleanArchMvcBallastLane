using CleanArchMvcBallastLane.API.Models.Commons;
using CleanArchMvcBallastLane.Domain.Validation;
using System.Net.Mime;
using System.Text.Json;

namespace CleanArchMvcBallastLane.API.Middlewares;
public sealed class ExceptionHandlerMiddleware(ILogger<ExceptionHandlerMiddleware> logger) : IMiddleware
{
    private static JsonSerializerOptions Options => new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            LogException(exception);
            await HandleExceptionAsync(context, exception);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = MediaTypeNames.Application.Json;
        context.Response.StatusCode = GetStatusCode(exception);

        var errorResponse = ErrorResponse
            .FromContext(context, exception)
            .WithErrors(GetErrorDetails(exception));
        var jsonErrorResponse = JsonSerializer.Serialize(errorResponse, Options);
        await context.Response.WriteAsync(jsonErrorResponse);
    }

    private void LogException(Exception exception)
    {
        if (exception is DomainExceptionValidation or ApplicationException)
        {
            logger.LogWarning(exception, "A business exception occurred: {exceptionMessage}", exception.Message);
            return;
        }

        logger.LogError(exception, "An unexpected error occurred while processing the request: '{exceptionMessage}'", exception.Message);
    }

    private static int GetStatusCode(Exception exception) =>
        exception switch
        {
            ArgumentNullException => StatusCodes.Status400BadRequest,
            ArgumentException => StatusCodes.Status400BadRequest,
            DomainExceptionValidation => StatusCodes.Status400BadRequest,
            ApplicationException => StatusCodes.Status400BadRequest,
            BadHttpRequestException => StatusCodes.Status400BadRequest,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };

    private static IEnumerable<ErrorDetail> GetErrorDetails(Exception exception) =>
        exception switch
        {
            DomainExceptionValidation or BadHttpRequestException or ApplicationException => [ErrorDetail.FromError(exception.Message)],
            _ => [ErrorDetail.InternalServerError]
        };
}