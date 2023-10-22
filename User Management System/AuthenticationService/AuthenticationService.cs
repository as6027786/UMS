using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Runtime.CompilerServices;
using User_Management_System.Controllers;
using User_Management_System.Models;

namespace User_Management_System.AuthenticationService
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ILogger<AuthenticationService> _logger;
        public AuthenticationService(ApplicationDbContext applicationDbContext, ILogger<AuthenticationService> logger)
        {
            _applicationDbContext = applicationDbContext;
            _logger = logger;
        }
        public async Task<bool> AuthenticateUserAsync(string usernameOrEmail, string password)
        {
            try
            {
                var user = await _applicationDbContext.ApplicationUser.FirstOrDefaultAsync(y => y.Username == usernameOrEmail || y.Email == usernameOrEmail);
                if (user != null)
                {
                    var result = BCrypt.Net.BCrypt.Verify(password, user.Password);
                    if (result)
                        return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, "User Not authenticated due to the following exception" + ex.Message);
                return false;
            }
        }
    }
}
