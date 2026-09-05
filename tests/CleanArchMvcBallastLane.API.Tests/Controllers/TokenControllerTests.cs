using CleanArchMvcBallastLane.API.Controllers;
using CleanArchMvcBallastLane.API.Models.Requests;
using CleanArchMvcBallastLane.API.Models.Responses;
using CleanArchMvcBallastLane.Application.Users.Authentication;
using CleanArchMvcBallastLane.Application.Users.Create;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CleanArchMvcBallastLane.API.Tests.Controllers;
public class TokenControllerTests
{
    private static readonly LoginRequest ValidLogin = new() { Email = "user@example.com", Password = "strong-password" };

    private static TokenController CreateController(Mock<IMediator> mediator)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:SecretKey"] = "this-is-a-test-secret-key-with-enough-length",
                ["Jwt:Issuer"] = "test-issuer",
                ["Jwt:Audience"] = "test-audience"
            })
            .Build();
        var controller = new TokenController(mediator.Object, configuration)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity([
                        new Claim(ClaimTypes.NameIdentifier, "user-123")
                    ], "Test"))
                }
            }
        };
        return controller;
    }

    [Fact]
    public async Task Should_ReturnOk_When_CreateUserCommandSucceeds()
    {
        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.Is<CreateUserCommand>(c => c.Email == ValidLogin.Email && c.Password == ValidLogin.Password), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await CreateController(mediator).CreateUser(ValidLogin);

        result.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be("User user@example.com was created successfully");
        mediator.Verify(x => x.Send(It.IsAny<CreateUserCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_ReturnBadRequest_When_CreateUserCommandFails()
    {
        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.IsAny<CreateUserCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await CreateController(mediator).CreateUser(ValidLogin);

        result.Should().BeOfType<BadRequestObjectResult>();
        mediator.Verify(x => x.Send(It.IsAny<CreateUserCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_ReturnValidJwt_When_AuthenticationSucceeds()
    {
        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.Is<AuthenticationCommand>(c => c.Email == ValidLogin.Email && c.Password == ValidLogin.Password), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await CreateController(mediator).Login(ValidLogin);

        var token = result.Value.Should().BeOfType<UserTokenResponse>().Subject;
        token.Token.Should().NotBeNullOrWhiteSpace();
        token.Expiration.Should().BeAfter(DateTime.UtcNow.AddMinutes(9));
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token.Token);
        jwt.Issuer.Should().Be("test-issuer");
        jwt.Audiences.Should().Contain("test-audience");
        jwt.Claims.Should().Contain(c => c.Type == ClaimTypes.Email && c.Value == ValidLogin.Email);
        jwt.Claims.Should().Contain(c => c.Type == ClaimTypes.NameIdentifier && c.Value == "user-123");
        jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Jti);
    }

    [Fact]
    public async Task Should_ReturnBadRequest_When_AuthenticationFails()
    {
        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.IsAny<AuthenticationCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await CreateController(mediator).Login(ValidLogin);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
        mediator.Verify(x => x.Send(It.IsAny<AuthenticationCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}