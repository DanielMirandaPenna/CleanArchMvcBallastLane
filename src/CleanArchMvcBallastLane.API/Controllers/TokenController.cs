using CleanArchMvcBallastLane.API.Models.Requests;
using CleanArchMvcBallastLane.API.Models.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CleanArchMvcBallastLane.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class TokenController(IMediator mediator, IConfiguration configuration) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("CreateUser")]
    public async Task<ActionResult> CreateUser([FromBody] LoginRequest userInfo)
    {
        var command = userInfo.ToCreateUserCommand();
        var result = await mediator.Send(command);

        if (result)
        {
            return Ok($"User {userInfo.Email} was created successfully");
        }

        ModelState.AddModelError(string.Empty, "Invalid Login attempt.");
        return BadRequest(ModelState);
    }

    [AllowAnonymous]
    [HttpPost("LoginUser")]
    public async Task<ActionResult<UserTokenResponse>> Login([FromBody] LoginRequest userInfo)
    {
        var command = userInfo.ToAuthenticationCommand();
        var result = await mediator.Send(command);
        if (result)
        {
            return GenerateToken(userInfo);
        }

        ModelState.AddModelError(string.Empty, "Invalid Login attempt.");
        return BadRequest(ModelState);
    }

    private UserTokenResponse GenerateToken(LoginRequest userInfo)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Email, userInfo.Email),
            new Claim(ClaimTypes.NameIdentifier, User.FindFirst(ClaimTypes.NameIdentifier)?.Value),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };


        var privateKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]));

        var credentials = new SigningCredentials(privateKey, SecurityAlgorithms.HmacSha256);
        var expiration = DateTime.UtcNow.AddMinutes(10);

        JwtSecurityToken token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: expiration,
            signingCredentials: credentials
            );

        return new UserTokenResponse()
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expiration = expiration
        };
    }
}