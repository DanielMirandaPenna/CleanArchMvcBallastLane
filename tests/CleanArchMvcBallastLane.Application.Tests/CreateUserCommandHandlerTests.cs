using CleanArchMvcBallastLane.Application.Users.Create;
using CleanArchMvcBallastLane.Domain.Account;
using FluentAssertions;
using Moq;

namespace CleanArchMvcBallastLane.Application.Tests;

public sealed class CreateUserCommandHandlerTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Should_ReturnRegistrationResult_When_CreateUserCommandIsHandled(bool expectedResult)
    {
        var authentication = new Mock<IAuthenticateService>();
        authentication.Setup(service => service.RegisterUser("user@example.com", "password")).ReturnsAsync(expectedResult);
        var handler = new CreateUserCommandHandler(authentication.Object);
        var command = new CreateUserCommand { Email = "user@example.com", Password = "password" };

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().Be(expectedResult);
        authentication.Verify(service => service.RegisterUser(command.Email, command.Password), Times.Once);
    }
}
