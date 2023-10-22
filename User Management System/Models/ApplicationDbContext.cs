using Microsoft.EntityFrameworkCore;
using System.Xml;

namespace User_Management_System.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the Username property as a unique key
            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(u => u.Username)
                .IsUnique();
        }
        public virtual DbSet<ApplicationUser> ApplicationUser { get; set; }

        public virtual DbSet<PasswordResetToken> UserPasswordResetToken { get; set; }
    }
}
