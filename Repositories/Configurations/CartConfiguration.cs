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
            entity.HasKey(c => c.Id);

            // Customer (1) -> (Many) Carts
            entity.HasOne(c => c.Customer)
                  .WithMany(cu => cu.Carts)
                  .HasForeignKey(c => c.CustomerId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Cart (1) -> (Many) CartItems
            entity.HasMany(c => c.Items)
                  .WithOne(ci => ci.Cart)
                  .HasForeignKey(ci => ci.CartId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Table name (optional)
            entity.ToTable("Carts");
        }
    }
}