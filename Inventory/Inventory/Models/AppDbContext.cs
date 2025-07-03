using Inventory.Models.ProductModel;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Product> Products { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ProductConfig());
            base.OnModelCreating(modelBuilder);
        }
    }
}
