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
            entity.HasKey(o => o.Id);

            // Customer (1) -> (Many) Orders
            entity.HasOne(o => o.Customer)
                  .WithMany(c => c.Orders)
                  .HasForeignKey(o => o.CustomerId)
                  .OnDelete(DeleteBehavior.Cascade);

            // OrderDetails (1) -> (Many) OrderDetail
            entity.HasMany(o => o.OrderDetails)
                  .WithOne(od => od.Order)
                  .HasForeignKey(od => od.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);

            // ShippingAddress Required
            entity.Property(o => o.ShippingAddress)
                  .IsRequired()
                  .HasMaxLength(300);

            // BillingAddress Required
            entity.Property(o => o.BillingAddress)
                  .IsRequired()
                  .HasMaxLength(300);

            // PaymentMethod Required
            entity.Property(o => o.PaymentMethod)
                  .IsRequired()
                  .HasMaxLength(100);

            // TotalAmount precision (optional recommendation)
            entity.Property(o => o.TotalAmount)
                  .HasColumnType("decimal(18,2)");

            // Status Enum
            entity.Property(o => o.Status)
                  .HasConversion<int>();

            // OrderDate Default Value
            entity.Property(o => o.OrderDate)
                  .HasDefaultValueSql("GETUTCDATE()");

            // Optional: Table name
            entity.ToTable("Orders");
        }
    }
}