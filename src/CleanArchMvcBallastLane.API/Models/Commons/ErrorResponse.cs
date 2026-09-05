using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace CleanArchMvcBallastLane.API.Models.Commons;

public sealed record ErrorResponse
{
    public string? Type { get; private set; }
    public string? Title { get; private set; }
    public string? Detail { get; private set; }
    public string Instance { get; private init; } = null!;
    public string TraceId { get; private init; } = null!;
    public IEnumerable<ErrorDetail> Errors { get; private set; } = null!;

    [JsonConstructor]
    public ErrorResponse(string instance,
    string traceId,
    IEnumerable<ErrorDetail> errors,
    string title,
    string detail)
    {
        Instance = instance;
        TraceId = traceId;
        Errors = errors;
        Title = title;
        Detail = detail;
    }

    protected ErrorResponse() { }

    public static ErrorResponse FromContext(HttpContext httpContext, Exception? exception = null)
    {
        return new ErrorResponse
        {
            TraceId = Activity.Current?.Id ?? httpContext.TraceIdentifier,
            Instance = httpContext.Request.Path.ToString(),
            Detail = exception?.Message,
            Type = exception?.GetType().Name,
            Title = "An error occurred",
        };
    }

    public ErrorResponse WithError(ErrorDetail errorDetail)
    {
        Errors = new[] { errorDetail };
        return this;
    }

    public ErrorResponse WithErrors(IEnumerable<ErrorDetail> errorDetails)
    {
        Errors = errorDetails;
        return this;
    }

    public ErrorResponse WithModelState(ModelStateDictionary modelState)
    {
        if (modelState.ValidationState == ModelValidationState.Valid)
        {
            return this;
        }

        var errors = modelState
            .Where(state => state.Value!.ValidationState == ModelValidationState.Invalid)
            .SelectMany(state => state.Value!.Errors.Select(ErrorDetail.FromModelError));

        WithErrors(errors);
        return this;
    }

}

public sealed record ErrorDetail
{
    public const string PayloadInvalid = nameof(PayloadInvalid);
    public const string DomainError = nameof(DomainError);
    public const string InternalError = nameof(InternalError);

    public required string Detail { get; init; }
    public string? Type { get; init; }
    public string? Error { get; init; }
    private const string UserErrorMessage = "An unexpected error occurred. Please try again later.";

    internal static ErrorDetail FromError(string error)
        => new()
        {
            Type = DomainError,
            Detail = error
        };

    internal static ErrorDetail InternalServerError
        => new()
        {
            Type = InternalError,
            Error = "Internal Server Error",
            Detail = UserErrorMessage
        };

    internal static ErrorDetail FromModelError(ModelError modelError)
        => new()
        {
            Type = "PayloadInvalid",
            Error = modelError.ErrorMessage,
            Detail = modelError.ErrorMessage,
        };
}
