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

            // Primary Key
            entity.HasKey(c => c.Id);

            // Name is required and max length
            entity.Property(c => c.Name)
                  .IsRequired()
                  .HasMaxLength(100);

            // Email optional but has a max length
            entity.Property(c => c.Email)
                  .HasMaxLength(150);

            // PhoneNumber optional but has a max length
            entity.Property(c => c.PhoneNumber)
                  .HasMaxLength(20);

            // ShippingAddress optional
            entity.Property(c => c.ShippingAddress)
                  .HasMaxLength(300);

            // BillingAddress optional
            entity.Property(c => c.BillingAddress)
                  .HasMaxLength(300);

            // DateCreated default value (optional)
            entity.Property(c => c.DateCreated)
                  .HasDefaultValueSql("GETUTCDATE()");

            // 1 Customer -> Many Orders
            entity.HasMany(c => c.Orders)
                  .WithOne(o => o.Customer)
                  .HasForeignKey(o => o.CustomerId)
                  .OnDelete(DeleteBehavior.Cascade);

            // 1 Customer -> Many Carts
            entity.HasMany(c => c.Carts)
                  .WithOne(ca => ca.Customer)
                  .HasForeignKey(ca => ca.CustomerId)
                  .OnDelete(DeleteBehavior.Cascade);

            // 1 Customer -> Many Wishlists
            entity.HasMany(c => c.Wishlists)
                  .WithOne(w => w.Customer)
                  .HasForeignKey(w => w.CustomerId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasKey(c => c.Id);

            entity.Property(c => c.Name).IsRequired();
            entity.Property(c => c.DateCreated).HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(c => c.ApplicationUser)
                   .WithMany()
                   .HasForeignKey(c => c.ApplicationUserId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Optional: Specify table name
            entity.ToTable("Customers");
        }
    }
}