using Dto;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> entity)
        {
            // Configure the primary key
            entity.HasKey(p => p.Id);

            // Configure properties
            entity.Property(p => p.Name)
                  .IsRequired()
                  .HasMaxLength(100); // Set maximum length for the product name

            entity.Property(p => p.Description)
                  .IsRequired()
                  .HasMaxLength(500); // Set maximum length for the description

            entity.Property(p => p.Price)
                  .IsRequired()
                  .HasColumnType("decimal(18,2)"); // Specify decimal precision and scale

            entity.Property(p => p.Stock)
                  .IsRequired(); // Ensure Stock is required


            entity.Property(p => p.ImageUrl)
                  .IsRequired(); // Ensure ImageUrl is required

            entity.Property(p => p.ImageUrl)
                  .IsRequired(); // Ensure ImageUrl is required

            // Map the 'CreateDate' property with default value set to the current time
            entity.Property(p => p.CreateDate)
                   .HasDefaultValueSql("GETDATE()")  // SQL Server default current date
                   .IsRequired();

            // Map the 'IsFeatured' property
            entity.Property(p => p.IsFeatured)
                   .IsRequired();

            // Since 'ImageFile' is not stored, we map the 'ImagePath' instead
            entity.Property(p => p.ImagePath)
                   .HasMaxLength(255)
                   .IsRequired(false);  // I



        
            entity.HasMany(p => p.ProductCategories)
                  .WithOne(pc => pc.Product) // Assuming ProductCategory has a navigation property to Product
                  .HasForeignKey(pc => pc.ProductId) // Foreign key on ProductCategory
                  .OnDelete(DeleteBehavior.Cascade); // Optional: Cascade delete when Product is deleted
            entity.Property(p => p.IsOutOfStock)
                 .IsRequired();
        }
    }
}