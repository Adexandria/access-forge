using AdeAuth.Models;
using Microsoft.EntityFrameworkCore;

namespace AdeAuth.Db
{
    /// <summary>
    /// Handles the db context for identity service 
    /// </summary>
    public class IdentityContext : DbContext 
    {
        public IdentityContext(DbContextOptions options) : base(options)
        {

        }

        public  DbSet<ApplicationRole> Roles { get; set; }

        public  DbSet<ApplicationUser> Users { get; set; }

        public  DbSet<UserRole> UserRoles { get; set; }

        public DbSet<LoginActivity> LoginActivities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationRole>()
                .HasMany<ApplicationUser>()
                .WithMany()
                .UsingEntity<UserRole>
                (l=> l.HasOne<ApplicationUser>().WithMany().HasForeignKey("UserId"),
                 r => r.HasOne<ApplicationRole>().WithMany().HasForeignKey("RoleId"));

            modelBuilder.Entity<ApplicationRole>().HasIndex(s=>s.Name).IsUnique();

            modelBuilder.Entity<ApplicationUser>().HasKey(s => s.Id);

            modelBuilder.Entity<ApplicationRole>().HasKey(s => s.Id);

            modelBuilder.Entity<LoginActivity>().HasKey(s=>s.Id);

            modelBuilder.Entity<LoginActivity>().HasIndex(s => s.IpAddress).IsUnique();

            modelBuilder.Entity<LoginActivity>().HasOne<ApplicationUser>()
                .WithMany().HasForeignKey(s=>s.UserId);
        }
    }

    /// <summary>
    /// Handles the db context for identity service 
    /// </summary>
    public class IdentityContext<TUser> : DbContext where TUser : ApplicationUser
    {
        public IdentityContext(DbContextOptions options): base(options)
        {
            
        }

        public  DbSet<TUser> Users { get; set; }

        public  DbSet<ApplicationRole> Roles { get; set; }

        public DbSet<UserRole> UserRoles { get; set; }

        public DbSet<LoginActivity> LoginActivities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationRole>()
                .HasMany<TUser>()
                .WithMany()
                .UsingEntity<UserRole>(l => l.HasOne<TUser>().WithMany().HasForeignKey("UserId"),
                 r => r.HasOne<ApplicationRole>().WithMany().HasForeignKey("RoleId"));

            modelBuilder.Entity<TUser>().HasKey(s => s.Id);

            modelBuilder.Entity<ApplicationRole>().HasKey(s => s.Id);

            modelBuilder.Entity<ApplicationRole>().HasIndex(s => s.Name).IsUnique();

            modelBuilder.Entity<LoginActivity>().HasKey(s => s.Id);

            modelBuilder.Entity<LoginActivity>().HasIndex(s => s.IpAddress).IsUnique();

            modelBuilder.Entity<LoginActivity>().HasOne<TUser>()
                .WithMany().HasForeignKey(s => s.UserId);
        }
    }


    /// <summary>
    /// Handles the db context for identity service 
    /// </summary>
    public class IdentityContext<TUser,TRole> : DbContext
        where TUser : ApplicationUser
        where TRole : ApplicationRole
    {
        public IdentityContext(DbContextOptions options):base(options)
        {

        }
        public  DbSet<TUser> Users { get; set; }
        public  DbSet<TRole> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        public DbSet<LoginActivity> LoginActivities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TRole>()
                .HasMany<TUser>()
                .WithMany()
                .UsingEntity<UserRole>(l => l.HasOne<TUser>().WithMany().HasForeignKey("UserId"),
                 r => r.HasOne<TRole>().WithMany().HasForeignKey("RoleId"));

            modelBuilder.Entity<TUser>().HasKey(s => s.Id);

            modelBuilder.Entity<TRole>().HasKey(s => s.Id);

            modelBuilder.Entity<TRole>().HasIndex(s => s.Name).IsUnique();

            modelBuilder.Entity<LoginActivity>().HasKey(s => s.Id);

            modelBuilder.Entity<LoginActivity>().HasIndex(s => s.IpAddress).IsUnique();

            modelBuilder.Entity<LoginActivity>().HasOne<TUser>()
                .WithMany().HasForeignKey(s => s.UserId);
        }
    }
}
