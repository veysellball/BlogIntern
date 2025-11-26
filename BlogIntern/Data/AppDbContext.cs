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

        public DbSet<UserWithRoleDto> UserWithRoleDtos => Set<UserWithRoleDto>();
        public DbSet<UserWithRoleSpDto> UserWithRoleSp => Set<UserWithRoleSpDto>();



        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        [Required]
        public DateTime InsertDate { get; set; } = DateTime.Now;

       
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserWithRoleDto>().HasNoKey();

            modelBuilder.Entity<UserWithRoleSpDto>().HasNoKey();

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
