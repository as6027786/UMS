namespace User_Management_System.AuthenticationService
{
    public interface IAuthenticationService
    {
        Task<bool> AuthenticateUserAsync(string username, string password);
    }
}
