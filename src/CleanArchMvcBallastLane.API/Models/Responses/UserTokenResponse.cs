namespace CleanArchMvcBallastLane.API.Models.Responses;

public class UserTokenResponse
{
    public string Token { get; init; }
    public DateTime Expiration { get; init; }
}
