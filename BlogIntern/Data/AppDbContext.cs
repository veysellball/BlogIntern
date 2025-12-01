using BlogIntern.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using BlogIntern.Dtos;

namespace BlogIntern.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<UserRoleSpDto> UserWithRoleDtos => Set<UserRoleSpDto>();
        public DbSet<UserRoleSpDto> UserWithRoleSp => Set<UserRoleSpDto>();



        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<ApiLog> ApiLogs { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }




        [Required]
        public DateTime InsertDate { get; set; } = DateTime.Now;

       
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserRoleSpDto>().HasNoKey();

            modelBuilder.Entity<UserRoleSpDto>().HasNoKey();

            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "User" }
            );


        }
    }
}
