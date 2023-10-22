using MessagePack;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using KeyAttribute = System.ComponentModel.DataAnnotations.KeyAttribute;

namespace User_Management_System.Models
{
    public class PasswordResetToken
    {
        [Key]
        public string Token { get; set; }
        public int UserId { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
