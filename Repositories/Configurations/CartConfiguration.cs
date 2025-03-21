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
    public class CartConfiguration : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> entity)
        {
            // Configure the primary key
            entity.HasKey(c => c.Id);

            // Configure the relationship between Cart and Customer (Many-to-One)
            entity.HasOne(c => c.Customer)
                  .WithMany(cust => cust.Carts) // Assuming Customer has a collection of Carts
                  .HasForeignKey(c => c.CustomerId)
                  .OnDelete(DeleteBehavior.Cascade); // Cascade delete when a Customer is deleted

            // Configure the relationship between Cart and CartItems (One-to-Many)
            entity.HasMany(c => c.Items)
                  .WithOne(ci => ci.Cart) // Assuming CartItem has a navigation property to Cart
                  .HasForeignKey(ci => ci.CartId)
                  .OnDelete(DeleteBehavior.Cascade); // Cascade delete when Cart is deleted

            // Optional: You can configure any default values or constraints if necessary
        }
    }
}