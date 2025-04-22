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

            // Payment → Order (Many-to-One)
            entity.HasOne(p => p.Order)
                  .WithMany()
                  .HasForeignKey(p => p.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);

            // PaymentMethod (optional) - MaxLength
            entity.Property(p => p.PaymentMethod)
                  .HasMaxLength(100);

            // IsPaid - Required
            entity.Property(p => p.IsPaid)
                  .IsRequired();

            // PaymentDate - Default Value UTC Now (optional)
            entity.Property(p => p.PaymentDate)
                  .HasDefaultValueSql("GETUTCDATE()");

            // Optional: Table name
            entity.ToTable("Payments");
        }
    }
}