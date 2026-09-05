namespace CleanArchMvcBallastLane.Domain.Account
{
    public interface IAuthenticateService
    {
        Task<bool> Authenticate(string email, string password);
        Task<bool> RegisterUser(string email, string password);
        Task Logout();
    }
}
