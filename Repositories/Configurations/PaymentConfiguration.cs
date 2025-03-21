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
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> entity)
        {
            // Configure the primary key
            entity.HasKey(p => p.Id);

            // Configure the foreign key relationship with Order
            entity.HasOne(p => p.Order)
                  .WithMany() // Assuming Order does not need to track Payments directly
                  .HasForeignKey(p => p.OrderId)
                  .OnDelete(DeleteBehavior.Cascade); // Optional: Cascade delete when Order is deleted

            // Configure properties
            entity.Property(p => p.PaymentMethod)
                  .HasMaxLength(50); // Optional: Set a maximum length for the payment method

            entity.Property(p => p.IsPaid)
                  .IsRequired(); // Ensure IsPaid is required

            entity.Property(p => p.PaymentDate)
                  .IsRequired(); // Ensure PaymentDate is required
        }
    }
}