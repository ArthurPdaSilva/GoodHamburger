using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Domain
{
    /// <summary>
    /// Entidade de configuração das entidades para o Banco de dados.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Order> Orders { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

       
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<MenuItem>().Property(b => b.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<Order>().Property(b => b.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}
