using Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repositories.Configurations
{

    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> entity)
        {
            // Configure the primary key
            entity.HasKey(c => c.Id);

            // Configure the Name property to be required with a maximum length
            entity.Property(c => c.Name)
                  .IsRequired()
                  .HasMaxLength(100); // Optional: max length of the category name
            entity.Property(c => c.Description)
                  .IsRequired()
                  .HasMaxLength(100);
            entity.Property(c => c.CreateDate)
                .IsRequired();

            // Configure the many-to-many relationship via ProductCategory
            entity.HasMany(c => c.ProductCategories)
                  .WithOne(pc => pc.Category)
                  .HasForeignKey(pc => pc.CategoryId)
                  .OnDelete(DeleteBehavior.Cascade); // Cascade delete when Category is deleted
        }
    }
}
