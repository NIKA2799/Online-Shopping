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
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> entity)
        {
            // Configure the primary key
            entity.HasKey(r => r.Id);

            // Rating (1-5)
            entity.Property(r => r.Rating)
                  .IsRequired();

            // Comment (optional)
            entity.Property(r => r.Comment)
                  .HasMaxLength(1000);

            // DatePosted (optional default)
            entity.Property(r => r.DatePosted)
                  .HasDefaultValueSql("GETUTCDATE()");

            // Product (1 Product → Many Reviews)
            entity.HasOne(r => r.Product)
                  .WithMany()
                  .HasForeignKey(r => r.ProductId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Customer (1 Customer → Many Reviews)
            entity.HasOne(r => r.Customer)
                  .WithMany()
                  .HasForeignKey(r => r.CustomerId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Optional Table Name
            entity.ToTable("Reviews");
        }
    }
}