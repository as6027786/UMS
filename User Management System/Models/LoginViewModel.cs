using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace User_Management_System.Models
{
    public class LoginViewModel
    {
            [Required(ErrorMessage = "Please enter your email or username.")]
            [Display(Name = "Email or Username")]
            public string EmailOrUsername { get; set; }

            [Required(ErrorMessage = "Please enter your password.")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember Me")]
            public bool RememberMe { get; set; }
            [HiddenInput]
            public string Role { get; set; }
    }

}

