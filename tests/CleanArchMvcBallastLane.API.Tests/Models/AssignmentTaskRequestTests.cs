using CleanArchMvcBallastLane.API.Models.Requests;
using CleanArchMvcBallastLane.Application.AssignmentTasks.Create;
using CleanArchMvcBallastLane.Application.AssignmentTasks.Update;
using CleanArchMvcBallastLane.Domain.Enums;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CleanArchMvcBallastLane.API.Tests.Models;
public class AssignmentTaskRequestTests
{
    [Fact]
    public void Should_MapAllFieldsAndUser_When_AssignmentTaskCreateRequestIsValid()
    {
        var request = new AssignmentTaskCreateRequest { Title = "Title", Description = "Description" };

        var command = request.ToCommand("user-1");

        command.Should().BeOfType<AssignmentTaskCreateCommand>();
        command.Title.Should().Be(request.Title);
        command.Description.Should().Be(request.Description);
        command.CreatedBy.Should().Be("user-1");
    }

    [Fact]
    public void Should_MapAllFieldsAndId_When_AssignmentTaskUpdateRequestIsValid()
    {
        var request = new AssignmentTaskUpdateRequest { Title = "Title", Description = "Description", Status = Status.Completed, CreatedBy = "user-1" };

        var command = request.ToCommand(4);

        command.Should().BeOfType<AssignmentTaskUpdateCommand>();
        command.Id.Should().Be(4);
        command.Title.Should().Be(request.Title);
        command.Description.Should().Be(request.Description);
        command.Status.Should().Be(request.Status);
        command.CreatedBy.Should().Be(request.CreatedBy);
    }

    [Theory]
    [InlineData("Title", "ab")]
    [InlineData("Description", "ab")]
    public void Should_RejectInvalidData_When_RequestModelContainsInvalidValue_AssignmentTaskCreateRequest(string propertyName, string? value)
    {
        object model = new AssignmentTaskCreateRequest
        {
            Title = propertyName == "Title" ? value! : "Valid title",
            Description = propertyName == "Description" ? value! : "Valid description"
        };

        var validation = new List<ValidationResult>();
        Validator.TryValidateObject(model, new ValidationContext(model), validation, true).Should().BeFalse();
        validation.Should().Contain(x => x.MemberNames.Contains(propertyName));
    }

    [Theory]
    [InlineData("Title", "ab")]
    [InlineData("Description", "ab")]
    public void Should_RejectInvalidData_When_RequestModelContainsInvalidValue_AssignmentTaskUpdateRequest(string propertyName, string? value)
    {
        object model = new AssignmentTaskUpdateRequest { Title = propertyName == "Title" ? value! : "Valid title", Description = propertyName == "Description" ? value! : "Valid description" };

        var validation = new List<ValidationResult>();
        Validator.TryValidateObject(model, new ValidationContext(model), validation, true).Should().BeFalse();
        validation.Should().Contain(x => x.MemberNames.Contains(propertyName));
    }

}