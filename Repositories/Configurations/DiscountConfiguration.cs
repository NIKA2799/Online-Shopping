using Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repositories.Configurations
{
    public class DiscountConfiguration : IEntityTypeConfiguration<Discount>
    {
        public void Configure(EntityTypeBuilder<Discount> entity)
        {
            // Configure the primary key
            entity.HasKey(d => d.Id);

            // Configure properties
            entity.Property(d => d.Code)
                  .IsRequired() // Optional: Make Code required
                  .HasMaxLength(50); // Optional: Set a maximum length for the discount code

            entity.Property(d => d.DiscountPercentage)
                  .IsRequired() // Optional: Make DiscountPercentage required
                  .HasColumnType("decimal(5,2)"); // Optional: Specify decimal precision and scale

            entity.Property(d => d.ExpirationDate)
                  .IsRequired(); // Optional: Make ExpirationDate required
        }
    }
}