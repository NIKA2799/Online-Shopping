using Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repositories.Configurations
{
    public class ShippingConfiguration : IEntityTypeConfiguration<Shipping>
    {
        public void Configure(EntityTypeBuilder<Shipping> entity)
        {
            // Configure the primary key
            entity.HasKey(s => s.Id);

            // Configure the foreign key relationship with Order
            entity.HasOne(s => s.Order)
                  .WithMany() // Assuming Order does not need to track Shipping records directly
                  .HasForeignKey(s => s.OrderId)
                  .OnDelete(DeleteBehavior.Cascade); // Optional: Cascade delete when Order is deleted

            // Configure properties
            entity.Property(s => s.ShippingAddress)
                  .HasMaxLength(255); // Optional: Set a maximum length for the shipping address

            entity.Property(s => s.ShippingMethod)
                  .HasMaxLength(50); // Optional: Set a maximum length for the shipping method

            entity.Property(s => s.ShippedDate)
                  .IsRequired(false); // Optional: Nullable property for ShippedDate

            entity.Property(s => s.DeliveryDate)
                  .IsRequired(false); // Optional: Nullable property for DeliveryDate
        }
    }
}