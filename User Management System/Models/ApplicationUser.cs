using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace User_Management_System.Models
{
    public class ApplicationUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Display(Name = "FullName")]
        public string FullName { get; set; }

        [Display(Name = "Username")]
        public string Username { get; set; }

        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; }
        [Display(Name = "Gender")]
        public string Gender { get; set; }
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [HiddenInput]
        public string Role { get; set; }
        [HiddenInput]
        public DateTime DateTime { get; set; }
    }
}

