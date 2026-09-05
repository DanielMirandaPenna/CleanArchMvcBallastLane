using CleanArchMvcBallastLane.Application.Users.Authentication;
using CleanArchMvcBallastLane.Domain.Account;
using FluentAssertions;
using Moq;

namespace CleanArchMvcBallastLane.Application.Tests;

public sealed class AuthenticationCommandHandlerTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Should_ReturnAuthenticationResult_When_AuthenticationCommandIsHandled(bool expectedResult)
    {
        var authentication = new Mock<IAuthenticateService>();
        authentication.Setup(service => service.Authenticate("user@example.com", "password")).ReturnsAsync(expectedResult);
        var handler = new AuthenticationCommandHandler(authentication.Object);
        var command = new AuthenticationCommand { Email = "user@example.com", Password = "password" };

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().Be(expectedResult);
        authentication.Verify(service => service.Authenticate(command.Email, command.Password), Times.Once);
    }

    [Fact]
    public async Task Should_PassAuthenticationCommandValues_When_AuthenticationIsHandled()
    {
        var authentication = new Mock<IAuthenticateService>();
        AuthenticationCommand? capturedCommand = null;
        authentication.Setup(service => service.Authenticate(It.IsAny<string>(), It.IsAny<string>()))
            .Callback<string, string>((email, password) => capturedCommand = new AuthenticationCommand { Email = email, Password = password })
            .ReturnsAsync(true);
        var handler = new AuthenticationCommandHandler(authentication.Object);
        var command = new AuthenticationCommand { Email = "captured@example.com", Password = "captured-password" };

        await handler.Handle(command, CancellationToken.None);

        capturedCommand.Should().BeEquivalentTo(command);
    }
}
