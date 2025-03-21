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
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> entity)
        {
            // Configure the primary key
            entity.HasKey(c => c.Id);

            // Configure required properties
            entity.Property(c => c.Name)
                  .IsRequired()
                  .HasMaxLength(100); // Optional: Set a maximum length

            entity.Property(c => c.Email)
                  .HasMaxLength(255); // Optional: Set a maximum length

            entity.Property(c => c.PhoneNumber)
                  .HasMaxLength(15); // Optional: Set a maximum length

            entity.Property(c => c.ShippingAddress)
                  .HasMaxLength(255); // Optional: Set a maximum length

            entity.Property(c => c.BillingAddress)
                  .HasMaxLength(255); // Optional: Set a maximum length

            entity.Property(c => c.DateCreated)
                  .HasDefaultValueSql("GETDATE()"); // Optional: Default value

            // Configure relationships with Orders and Carts if needed
            entity.HasMany(c => c.Orders)
                  .WithOne() // Assuming Order has a navigation property to Customer
                  .HasForeignKey(o => o.CustomerId) // Assuming Order has CustomerId property
                  .OnDelete(DeleteBehavior.Cascade); // Cascade delete

            entity.HasMany(c => c.Carts)
                  .WithOne() // Assuming Cart has a navigation property to Customer
                  .HasForeignKey(c => c.CustomerId) // Assuming Cart has CustomerId property
                  .OnDelete(DeleteBehavior.Cascade); // Cascade delete
        }
    }
}