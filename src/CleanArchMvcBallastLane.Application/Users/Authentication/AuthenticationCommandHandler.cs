using CleanArchMvcBallastLane.Domain.Account;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchMvcBallastLane.Application.Users.Authentication;
public class AuthenticationCommandHandler(IAuthenticateService authentication) : IRequestHandler<AuthenticationCommand, bool>
{
    public async Task<bool> Handle(AuthenticationCommand request, CancellationToken cancellationToken)
    {
        var result = await authentication.Authenticate(request.Email, request.Password);
        return result;
    }
}