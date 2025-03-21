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
    public class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
    {
        public void Configure(EntityTypeBuilder<ProductCategory> entity)
        {
            // Composite primary key (ProductId + CategoryId)
            entity.HasKey(pc => new { pc.ProductId, pc.CategoryId });

            // Configure the relationship between ProductCategory and Product
            entity.HasOne(pc => pc.Product)
                  .WithMany(p => p.ProductCategories)
                  .HasForeignKey(pc => pc.ProductId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Configure the relationship between ProductCategory and Category
            entity.HasOne(pc => pc.Category)
                  .WithMany(c => c.ProductCategories)
                  .HasForeignKey(pc => pc.CategoryId)
                  .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
