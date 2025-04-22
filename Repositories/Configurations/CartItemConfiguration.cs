using Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repositories.Configurations
{
    public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> entity)
        {
            // Primary Key
            entity.HasKey(ci => ci.Id);

            // Cart (1) -> (Many) CartItems
            entity.HasOne(ci => ci.Cart)
                  .WithMany(c => c.Items)
                  .HasForeignKey(ci => ci.CartId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Product (1) -> (Many) CartItems
            entity.HasOne(ci => ci.Product)
                  .WithMany()
                  .HasForeignKey(ci => ci.ProductId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Optional: MaxLength constraint for Items (if you keep it)
            if (typeof(CartItem).GetProperty("Items") != null)
            {
                entity.Property(ci => ci.Items)
                      .HasMaxLength(200); // Example: Limit text length
            }

            entity.ToTable("CartItems");
        }
    }
}
