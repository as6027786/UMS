using NuGet.Common;
using User_Management_System.Models;

namespace User_Management_System.Repositories
{
    public interface IRepository
    {
        Task<int> CreateNewUser(ApplicationUser applicationUser);
        void SendEmail(string ToEmail, string link);
        bool SaveTokenInDatabase(PasswordResetToken passwordResetToken);
        ApplicationUser FindUserByEmailAsync(string email);
        PasswordResetToken GetPasswordResetTokenByToken(string token);
        bool ResetUserAccountPassword(ResetPasswordModel resetPasswordModel);
        string GetUserRole(string UsernameOrEmail);
    }
}
