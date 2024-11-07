using AdeAuth.Client.Models;
using AdeAuth.Db;
using Microsoft.EntityFrameworkCore;

namespace AdeAuth.Client.Db
{
    public class IdentityDbContext : IdentityContext<User, Role>
    {
        public IdentityDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().ToTable("Users");

            modelBuilder.Entity<User>().HasOne(u => u.Role)
                .WithMany(s => s.Users).HasForeignKey(s=>s.RoleId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Role>().ToTable("Roles");
        }
    }
}
