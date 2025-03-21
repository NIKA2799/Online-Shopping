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
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> entity)
        {
            // Configure the primary key
            entity.HasKey(o => o.Id);

            // Configure the foreign key relationship with Customer
            entity.HasOne(o => o.Customer)
                  .WithMany(c => c.Orders) // Assuming Customer has a collection of Orders
                  .HasForeignKey(o => o.CustomerId)
                  .OnDelete(DeleteBehavior.Cascade); // Optional: Cascade delete when Customer is deleted

            // Configure properties
            entity.Property(o => o.OrderDate)
                  .IsRequired(); // Ensure OrderDate is required

            entity.Property(o => o.TotalAmount)
                  .IsRequired() // Ensure TotalAmount is required
                  .HasColumnType("decimal(18,2)"); // Optional: Specify decimal precision and scale

            // Configure relationship with OrderDetails
            entity.HasMany(o => o.OrderDetails)
                  .WithOne(od => od.Order) // Assuming OrderDetail has a navigation property to Order
                  .HasForeignKey(od => od.OrderId) // Assuming OrderDetail has OrderId property
                  .OnDelete(DeleteBehavior.Cascade); // Optional: Cascade delete when Order is deleted

            entity.Property(o => o.ShippingAddress)
              .HasMaxLength(255)
              .IsRequired(false);  // Optional

            // Configure the BillingAddress property
            entity.Property(o => o.BillingAddress)
                   .HasMaxLength(255)
                   .IsRequired(false);  // Optional

            // Configure the PaymentMethod property
            entity.Property(o => o.PaymentMethod)
                   .HasMaxLength(50)
                   .IsRequired(false);  // Optiona
        }
    }
}