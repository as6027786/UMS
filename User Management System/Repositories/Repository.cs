using System.Net.Mail;
using System.Net;
using User_Management_System.Models;
using System.Text;
using Microsoft.EntityFrameworkCore;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace User_Management_System.Repositories
{
    public class Repository : IRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ILogger<Repository> _logger;
        private readonly IConfiguration _configuration;

        public Repository(ApplicationDbContext applicationDbContext, ILogger<Repository> logger, IConfiguration configuration)
        {
            _applicationDbContext = applicationDbContext;
            _logger = logger;
            _configuration = configuration;
        }

        public bool SaveTokenInDatabase(PasswordResetToken passwordResetToken)
        {
            try
            {
                _applicationDbContext.UserPasswordResetToken.Add(passwordResetToken);
                _applicationDbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, "unable to save the token in the Database due to the following error" + ex.Message);
                return false;
            }
        }

        public async Task<int> CreateNewUser(ApplicationUser applicationUser)
        {
            try
            {
                var user = await _applicationDbContext.ApplicationUser.FirstOrDefaultAsync(user => user.Username == applicationUser.Username || user.Email == applicationUser.Email);
                if (user != null)
                {
                    return 0;
                }
                var plainPassword = applicationUser.Password;
                applicationUser.Password = BCrypt.Net.BCrypt.HashPassword(plainPassword);
                applicationUser.DateTime = DateTime.Now;
                await _applicationDbContext.ApplicationUser.AddAsync(applicationUser);
                var res = await _applicationDbContext.SaveChangesAsync();
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ApplicationUser FindUserByEmailAsync(string email)
        {
            ApplicationUser applicationUser = new ApplicationUser();
            applicationUser = _applicationDbContext.ApplicationUser.Where(x => x.Email == email)?.FirstOrDefault();
            return applicationUser;
        }

        public PasswordResetToken GetPasswordResetTokenByToken(string token)
        {
            PasswordResetToken newToken = new PasswordResetToken();
            newToken = _applicationDbContext.UserPasswordResetToken.Where(x => x.Token == token).FirstOrDefault();
            return newToken;
        }

        public bool ResetUserAccountPassword(ResetPasswordModel resetPasswordModel)
        {
            try
            {
                var user = _applicationDbContext.ApplicationUser.FirstOrDefault(x => x.Id == resetPasswordModel.Id);
                if (user != null)
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(resetPasswordModel.NewPassword);
                    _applicationDbContext.SaveChanges();
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, "Getting error when change password for the user due to the error : " + ex.Message);
                return false;
            }
        }

        public void SendEmail(string ToEmail , string PasswordResetlink)
        {
            StringBuilder stringBuilder = new StringBuilder();
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("as6027786@gmail.com");
            mailMessage.To.Add(ToEmail);
            mailMessage.Priority = MailPriority.High;
            mailMessage.Subject = "Reset your Password";
            stringBuilder.Append(@"<html>
                      <body>
                      <p>Dear User,</p>
					  <p>Hope you are doing well.</p>
                      <p>Please follow the below link to reset your password</p>
                      </body>
                      </html>
                     ");
            stringBuilder.Append(PasswordResetlink);

            //Todo : Here We save the Guid into the Database to verify that the correct user access.
            try
            {
                string htmlString = stringBuilder.ToString();
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = htmlString;
                SmtpClient smtpClient = new SmtpClient();
                smtpClient.Host = _configuration["EmailSettings:SmtpServer"];
                smtpClient.Port = 587;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(_configuration["EmailSettings:UserName"], _configuration["EmailSettings:Password"]);
                smtpClient.EnableSsl = true;

                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
               _logger.Log(LogLevel.Error,"Error while sending email: " + ex.Message);
            }
        }

        public string GetUserRole(string UsernameOrEmail)
        {
            var role = _applicationDbContext.ApplicationUser.Where(x => x.Username == UsernameOrEmail || x.Email == UsernameOrEmail).FirstOrDefault();
            if (!string.IsNullOrEmpty(role?.Role))
                return role.Role;
            return null;
        }
    }   
}
