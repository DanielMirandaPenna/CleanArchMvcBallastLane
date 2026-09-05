using CleanArchMvcBallastLane.Application.Users.Authentication;
using CleanArchMvcBallastLane.Application.Users.Create;
using System.ComponentModel.DataAnnotations;

namespace CleanArchMvcBallastLane.API.Models.Requests;
public class LoginRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid format email")]
    public string? Email { get; init; }

    [Required(ErrorMessage = "Password is required")]
    [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at max " +
        "{1} characters long.", MinimumLength = 10)]
    [DataType(DataType.Password)]
    public string Password { get; init; }

    public AuthenticationCommand ToAuthenticationCommand()
    {
        return new AuthenticationCommand()
        {
            Email = Email,
            Password = Password
        };
    }

    public CreateUserCommand ToCreateUserCommand()
    {
        return new CreateUserCommand()
        {
            Email = Email,
            Password = Password
        };
    }
}