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
    public class OrderDetailConfiguration : IEntityTypeConfiguration<OrderDetail>
    {
        public void Configure(EntityTypeBuilder<OrderDetail> entity)
        {
            // Configure the primary key
            entity.HasKey(od => od.Id);

            // Configure the foreign key relationship with Order
            entity.HasOne(od => od.Order)
                  .WithMany(o => o.OrderDetails) // Assuming Order has a collection of OrderDetails
                  .HasForeignKey(od => od.OrderId)
                  .OnDelete(DeleteBehavior.Cascade); // Optional: Cascade delete when Order is deleted

            // Configure the foreign key relationship with Product
            entity.HasOne(od => od.Product)
                  .WithMany() // Assuming Product does not need to track OrderDetails directly
                  .HasForeignKey(od => od.ProductId)
                  .OnDelete(DeleteBehavior.Restrict); // Optional: Restrict deletion of Product if it's linked to OrderDetails

            // Configure properties
            entity.Property(od => od.Quantity)
                  .IsRequired(); // Ensure Quantity is required

            entity.Property(od => od.UnitPrice)
                  .IsRequired() // Ensure UnitPrice is required
                  .HasColumnType("decimal(18,2)"); // Optional: Specify decimal precision and scale
        }
    }
}