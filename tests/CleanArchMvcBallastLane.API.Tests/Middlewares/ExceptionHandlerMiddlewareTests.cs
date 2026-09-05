using CleanArchMvcBallastLane.API.Middlewares;
using CleanArchMvcBallastLane.Domain.Validation;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace CleanArchMvcBallastLane.API.Tests.Middlewares;
public class ExceptionHandlerMiddlewareTests
{
    [Theory]
    [InlineData(typeof(ArgumentException), 400, "An unexpected error occurred. Please try again later.", LogLevel.Error)]
    [InlineData(typeof(ArgumentNullException), 400, "An unexpected error occurred. Please try again later.", LogLevel.Error)]
    [InlineData(typeof(BadHttpRequestException), 400, "invalid request", LogLevel.Error)]
    [InlineData(typeof(DomainExceptionValidation), 400, "domain failure", LogLevel.Warning)]
    [InlineData(typeof(ApplicationException), 400, "application failure", LogLevel.Warning)]
    [InlineData(typeof(InvalidOperationException), 500, "An unexpected error occurred. Please try again later.", LogLevel.Error)]
    public async Task Should_ReturnMappedErrorResponse_When_NextThrowsException(
        Type exceptionType,
        int expectedStatus,
        string expectedDetail,
        LogLevel expectedLogLevel)
    {
        var logger = new Mock<ILogger<ExceptionHandlerMiddleware>>();
        var middleware = new ExceptionHandlerMiddleware(logger.Object);
        var context = new DefaultHttpContext();
        context.TraceIdentifier = "trace-1";
        context.Request.Path = "/api/tasks";
        context.Response.Body = new MemoryStream();
        var exception = CreateException(exceptionType);

        await middleware.InvokeAsync(context, _ => throw exception);

        context.Response.StatusCode.Should().Be(expectedStatus);
        context.Response.ContentType.Should().Be("application/json");
        context.Response.Body.Position = 0;
        var response = await JsonSerializer.DeserializeAsync<JsonElement>(context.Response.Body);
        response.GetProperty("traceId").GetString().Should().Be("trace-1");
        response.GetProperty("instance").GetString().Should().Be("/api/tasks");
        response.GetProperty("errors")[0].GetProperty("detail").GetString().Should().Be(expectedDetail);

        var logInvocation = logger.Invocations
            .Where(invocation => invocation.Method.Name == nameof(ILogger.Log))
            .Should()
            .ContainSingle()
            .Subject;

        logInvocation.Arguments[0].Should().Be(expectedLogLevel);
        logInvocation.Arguments[3].Should().BeSameAs(exception);

        var expectedLogTemplate = expectedLogLevel == LogLevel.Warning
            ? "A business exception occurred: {exceptionMessage}"
            : "An unexpected error occurred while processing the request: '{exceptionMessage}'";
        var logState = logInvocation.Arguments[2]
            .Should()
            .BeAssignableTo<IReadOnlyList<KeyValuePair<string, object?>>>()
            .Subject;

        logState.Should().Contain(logEntry =>
            logEntry.Key == "{OriginalFormat}" &&
            logEntry.Value != null &&
            logEntry.Value.ToString() == expectedLogTemplate);

        var expectedLogMessage = expectedLogTemplate.Replace("{exceptionMessage}", exception.Message);
        logState.ToString().Should().Be(expectedLogMessage);
    }

    [Fact]
    public async Task Should_LeaveResponseUntouched_When_NextSucceeds()
    {
        var logger = new Mock<ILogger<ExceptionHandlerMiddleware>>();
        var middleware = new ExceptionHandlerMiddleware(logger.Object);
        var context = new DefaultHttpContext();
        var next = new Mock<RequestDelegate>();

        await middleware.InvokeAsync(context, next.Object);

        context.Response.StatusCode.Should().Be(200);
        context.Response.ContentType.Should().BeNull();
        next.Verify(x => x(It.IsAny<HttpContext>()), Times.Once);
        logger.VerifyNoOtherCalls();
    }

    private static Exception CreateException(Type type) => type switch
    {
        var t when t == typeof(ArgumentException) => new ArgumentException("argument failure"),
        var t when t == typeof(ArgumentNullException) => new ArgumentNullException("value"),
        var t when t == typeof(BadHttpRequestException) => new BadHttpRequestException("invalid request"),
        var t when t == typeof(DomainExceptionValidation) => new DomainExceptionValidation("domain failure"),
        var t when t == typeof(ApplicationException) => new ApplicationException("application failure"),
        _ => new InvalidOperationException("unexpected failure")
    };
}