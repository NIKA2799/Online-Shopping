using Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repositories.Configurations
{

    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> entity)
        {
            entity.HasKey(c => c.Id);

            // Name is required and has a maximum length
            entity.Property(c => c.Name)
                  .IsRequired()
                  .HasMaxLength(100); 

            // Description is required and has a maximum length
            entity.Property(c => c.Description)
                  .IsRequired()
                  .HasMaxLength(500); 

            // CreateDate - default value current time
            entity.Property(c => c.CreateDate)
                  .HasDefaultValueSql("GETUTCDATE()");

            // Relationship with ProductCategories
            entity.HasMany(c => c.ProductCategories)
                  .WithOne(pc => pc.Category)
                  .HasForeignKey(pc => pc.CategoryId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Optional: Table name (if you want)
            entity.ToTable("Categories");
        }
    }
}
