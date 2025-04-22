using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repositories.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> entity)
        {

            // Primary Key
            entity.HasKey(p => p.Id);

            // Name
            entity.Property(p => p.Name)
                  .IsRequired()
                  .HasMaxLength(200);

            // Description
            entity.Property(p => p.Description)
                  .IsRequired()
                  .HasMaxLength(1000);

            // Price
            entity.Property(p => p.Price)
                  .HasColumnType("decimal(18,2)")
                  .IsRequired();

            // Stock
            entity.Property(p => p.Stock)
                  .IsRequired();

            // ImageUrl
            entity.Property(p => p.ImageUrl)
                  .IsRequired()
                  .HasMaxLength(300);

            // ImagePath
            entity.Property(p => p.ImagePath)
                  .HasMaxLength(300);

            // ImageFile (ignored - not mapped to database)
            entity.Ignore(p => p.ImageFile);

            // CreateDate
            entity.Property(p => p.CreateDate)
                  .HasDefaultValueSql("GETUTCDATE()");

            // IsFeatured
            entity.Property(p => p.IsFeatured)
                  .HasDefaultValue(false);

            // IsOutOfStock
            entity.Property(p => p.IsOutOfStock)
                  .HasDefaultValue(false);

            // Items (optional field)
            entity.Property(p => p.Items)
                  .HasMaxLength(500);

            // Relationship with ProductCategories
            entity.HasMany(p => p.ProductCategories)
                  .WithOne(pc => pc.Product)
                  .HasForeignKey(pc => pc.ProductId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Table Name (optional)
            entity.ToTable("Products");

        }
    }
}