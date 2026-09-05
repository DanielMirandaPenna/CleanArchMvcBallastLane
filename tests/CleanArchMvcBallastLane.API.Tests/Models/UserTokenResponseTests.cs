using CleanArchMvcBallastLane.API.Models.Responses;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchMvcBallastLane.API.Tests.Models;
public class UserTokenResponseTests
{
    [Fact]
    public void Should_ExposeTokenAndExpiration_When_UserTokenIsCreated()
    {
        var expiration = DateTime.UtcNow.AddMinutes(10);
        var token = new UserTokenResponse { Token = "jwt", Expiration = expiration };

        token.Token.Should().Be("jwt");
        token.Expiration.Should().Be(expiration);
    }
}