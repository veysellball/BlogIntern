using BlogIntern.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using BlogIntern.Dtos;

namespace BlogIntern.Data
{
    public class AdventureContext : DbContext
    {
        public AdventureContext(DbContextOptions<AdventureContext> options)
            : base(options)
        {
        }

        public DbSet<SalesOrderDetail> SalesOrderDetails { get; set; }
    }
}
