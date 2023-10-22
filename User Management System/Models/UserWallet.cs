using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace User_Management_System.Models
{
    public class UserWallet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserWalletId { get; set; }
        [Required]
        public int UserId { get; set; }

        [ForeignKey("Id")]
        public ApplicationUser User { get; set; }
        public decimal Balance { get; set; } = 500;
    }
}
