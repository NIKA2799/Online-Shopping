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
    public class WishlistConfiguration : IEntityTypeConfiguration<Wishlist>
    {
        public void Configure(EntityTypeBuilder<Wishlist> entity)
        {
            // Configure the primary key
            entity.HasKey(w => w.Id);

            // Configure the foreign key relationship with Customer
            entity.HasOne(w => w.Customer)
                  .WithMany(c => c.Wishlists) // Assuming Customer has a collection of Wishlists
                  .HasForeignKey(w => w.CustomerId)
                  .OnDelete(DeleteBehavior.Cascade); // Optional: Cascade delete when Customer is deleted

            // Configure properties
            entity.Property(w => w.CustomerId)
                  .IsRequired(); // Ensure CustomerId is required

            // Configure the relationship with WishlistItem
            entity.HasMany(w => w.Items)
                  .WithOne() // Assuming WishlistItem does not need to track Wishlists directly
                  .HasForeignKey(wi => wi.WishlistId) // Assuming WishlistItem has WishlistId property
                  .OnDelete(DeleteBehavior.Cascade); // Optional: Cascade delete when Wishlist is deleted
        }
    }
}