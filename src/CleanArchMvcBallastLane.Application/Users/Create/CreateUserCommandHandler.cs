using CleanArchMvcBallastLane.Domain.Account;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchMvcBallastLane.Application.Users.Create;

public class CreateUserCommandHandler(IAuthenticateService authentication) : IRequestHandler<CreateUserCommand, bool>
{
    public async Task<bool> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var result = await authentication.RegisterUser(request.Email, request.Password);
        return result;
    }
}