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

    public class WishlistItemConfiguration : IEntityTypeConfiguration<WishlistItem>
    {
        public void Configure(EntityTypeBuilder<WishlistItem> entity)
        {
            // Configure the primary key
            entity.HasKey(wi => wi.Id);

            // Configure the foreign key relationship with Wishlist
            entity.HasOne(wi => wi.Wishlist)
                  .WithMany(w => w.Items) // Assuming Wishlist has a collection of WishlistItems
                  .HasForeignKey(wi => wi.WishlistId)
                  .OnDelete(DeleteBehavior.Cascade); // Optional: Cascade delete when Wishlist is deleted

            // Configure the foreign key relationship with Product
            entity.HasOne(wi => wi.Product)
                  .WithMany() // Assuming Product does not need to track WishlistItems directly
                  .HasForeignKey(wi => wi.ProductId)
                  .OnDelete(DeleteBehavior.Restrict); // Optional: Restrict deletion of Product if it's linked to WishlistItems
        }
    }
}