using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchMvcBallastLane.Application.Users.Authentication;
public record AuthenticationCommand : IRequest<bool>
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}