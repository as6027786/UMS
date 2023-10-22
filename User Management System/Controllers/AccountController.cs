using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using User_Management_System.Models;
using IAuthenticationService = User_Management_System.AuthenticationService.IAuthenticationService;
using static System.Net.Mime.MediaTypeNames;
using User_Management_System.Repositories;
using Microsoft.AspNetCore.Identity;
using User_Management_System.AuthenticationService;
using NuGet.Common;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Authorization;

namespace User_Management_System.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthenticationService _authenticationService;

        private readonly ILogger<AccountController> _logger;

        private readonly IRepository _repository;
        public AccountController(IAuthenticationService authenticationService, ILogger<AccountController> logger, IRepository repository)
        {
            _authenticationService = authenticationService;
            _logger = logger;
            _repository = repository;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel { Role = "User"});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                string UserRole = _repository.GetUserRole(model.EmailOrUsername);

                if (await _authenticationService.AuthenticateUserAsync(model.EmailOrUsername, model.Password))
                {
                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, model.EmailOrUsername),
                    new Claim(ClaimTypes.Role,UserRole ?? model.Role)
                };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    HttpContext.Session.SetString("User", model.EmailOrUsername);
    
                    return RedirectToAction("Index", "Home"); // Redirect to the main page after successful login
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        public IActionResult SignUpPage()
        {
            return View(new ApplicationUser { Role = "User"});
        }

        public async Task<IActionResult> SignUp(ApplicationUser applicationUser)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (await _repository.CreateNewUser(applicationUser) > 0)
                        return RedirectToAction("Login");
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, "Getting Error while Signing up due to the following exception" + ex, new List<object> { applicationUser.Email, applicationUser.Username });
            }
            return RedirectToAction("SignUp");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _repository.FindUserByEmailAsync(model.Email);
                if (user != null)
                {
                    string token = TokenGenerator.GenerateRandomToken(20); 
                    DateTime expirationDate = DateTime.Now.AddMinutes(30);

                    var passwordResetToken = new PasswordResetToken
                    {
                        UserId = user.Id,
                        Token = token,
                        ExpirationDate = expirationDate
                    };

                    string resetLink = "https://localhost:7179/Account/ResetPassword?" + token + "?" + user.Id;

                    try
                    {
                        _repository.SaveTokenInDatabase(passwordResetToken);
                        _repository.SendEmail(user.Email, resetLink);
                        return Content("Password reset email Successfully send to the user EmailId - " + user.Email);
                    }
                    catch (Exception ex)
                    {
                        _logger.Log(LogLevel.Error, "Password reset link not send to the user Due to the following exception " + ex.Message);
                        return View("Error");
                    }                    
                }
                else
                {
                    return Content("User Not found");
                }
            }
            return View(model);
        }
        public IActionResult ResetPassword()
        {
            try
            {
                string token = HttpContext.Request.QueryString.Value;
                var CorrectToken = token?.Substring(1);
                var arr = CorrectToken.Split('?');
                int UserId = int.Parse(arr[1]);
                var passwordResetToken = _repository.GetPasswordResetTokenByToken(arr[0]);
                if (passwordResetToken != null && passwordResetToken.ExpirationDate >= DateTime.Now)
                {
                    ResetPasswordModel resetPasswordModel = new ResetPasswordModel();
                    resetPasswordModel.Id = UserId;

                    return View("ForgotPasswordPageView", resetPasswordModel);

                }
                else
                {
                    return Content("Token is not valid or expired");
                }
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }

        [HttpPost]
        public IActionResult UpdatePassword(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var response = _repository.ResetUserAccountPassword(model);
                if (response == true)
                {
                    return Content("User Password reset successfully");
                }
                else
                {
                    return Content("User password not reset due to some issue");
                }
            }
            return View(model);
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLogin(string returnUrl = null, string remoteError = null)
        {
            throw new NotImplementedException();
        }

    }
}

