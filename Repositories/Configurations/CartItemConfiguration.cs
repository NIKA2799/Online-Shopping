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
    public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> entity)
        {
            // Configure the primary key
            entity.HasKey(ci => ci.Id);

            // Configure the Cart relationship (Many-to-One)
            entity.HasOne(ci => ci.Cart)
                  .WithMany(c => c.Items) // Assuming Cart has a collection of CartItems
                  .HasForeignKey(ci => ci.CartId)
                  .OnDelete(DeleteBehavior.Cascade); // Cascade delete when Cart is deleted


            // Configure Quantity as a required field with a default value
            entity.Property(ci => ci.Quantity)
                  .IsRequired()
                  .HasDefaultValue(1); // Default value of 1 for quantity
        }
    }
}
