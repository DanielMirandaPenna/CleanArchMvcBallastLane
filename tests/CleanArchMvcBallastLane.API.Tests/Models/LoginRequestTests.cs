using CleanArchMvcBallastLane.API.Models.Requests;
using CleanArchMvcBallastLane.Application.Users.Authentication;
using CleanArchMvcBallastLane.Application.Users.Create;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CleanArchMvcBallastLane.API.Tests.Models;
public class LoginRequestTests
{
    [Fact]
    public void Should_MapToAuthenticationCommand_When_LoginModelIsValid()
    {
        var model = new LoginRequest { Email = "user@example.com", Password = "strong-password" };

        var command = model.ToAuthenticationCommand();

        command.Should().BeOfType<AuthenticationCommand>();
        command.Email.Should().Be(model.Email);
        command.Password.Should().Be(model.Password);
    }

    [Fact]
    public void Should_MapToCreateUserCommand_When_LoginModelIsValid()
    {
        var model = new LoginRequest { Email = "user@example.com", Password = "strong-password" };

        var command = model.ToCreateUserCommand();

        command.Should().BeOfType<CreateUserCommand>();
        command.Email.Should().Be(model.Email);
        command.Password.Should().Be(model.Password);
    }

    [Theory]
    [InlineData("Email", null)]
    [InlineData("Email", "invalid")]
    [InlineData("Password", "short")]
    public void Should_RejectInvalidData_When_RequestModelContainsInvalidValue(string propertyName, string? value)
    {
        object model = new LoginRequest { Email = propertyName == "Email" ? value : "user@example.com", Password = propertyName == "Password" ? value! : "strong-password" };

        var validation = new List<ValidationResult>();
        Validator.TryValidateObject(model, new ValidationContext(model), validation, true).Should().BeFalse();
        validation.Should().Contain(x => x.MemberNames.Contains(propertyName));
    }
}