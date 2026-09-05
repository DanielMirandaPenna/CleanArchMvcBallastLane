using CleanArchMvcBallastLane.API.Models.Commons;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchMvcBallastLane.API.Tests.Models;
public class ErrorResponseTests
{
    [Fact]
    public void Should_BuildFromContextAndSetErrors_When_ErrorResponseIsConfigured()
    {
        var context = new DefaultHttpContext();
        context.TraceIdentifier = "trace-1";
        context.Request.Path = "/api/tasks";
        var exception = new InvalidOperationException("failure");

        var response = ErrorResponse.FromContext(context, exception)
            .WithError(new ErrorDetail { Type = "Test", Detail = "detail" });

        response.TraceId.Should().Be("trace-1");
        response.Instance.Should().Be("/api/tasks");
        response.Type.Should().Be(nameof(InvalidOperationException));
        response.Detail.Should().Be("failure");
        response.Errors.Should().ContainSingle().Which.Detail.Should().Be("detail");

        var multiple = ErrorResponse.FromContext(context).WithErrors([
            new ErrorDetail { Type = "Test", Detail = "first" },
            new ErrorDetail { Type = "Test", Detail = "second" }
        ]);
        multiple.Errors.Should().HaveCount(2);
    }

    [Fact]
    public void Should_ConvertInvalidModelStateErrors_When_ModelStateIsInvalid()
    {
        var context = new DefaultHttpContext();
        var modelState = new ModelStateDictionary();
        modelState.AddModelError("Title", "Title is required");

        var response = ErrorResponse.FromContext(context).WithModelState(modelState);

        response.Errors.Should().ContainSingle().Which.Error.Should().Be("Title is required");
    }
}