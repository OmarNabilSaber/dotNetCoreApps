using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Models.ProductModel
{
    public class ProductConfig : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Product");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Code)
                   .IsRequired()
                   .HasMaxLength(20);
            builder.HasIndex(p => p.Code)
                   .IsUnique();
            builder.Property(p => p.Quantity).HasDefaultValue(0);
            builder.Property(p => p.Name).HasMaxLength(100);
        }
    }
}
