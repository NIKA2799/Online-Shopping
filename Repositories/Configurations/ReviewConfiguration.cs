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

            // Configure foreign key relationships
            entity.HasOne(r => r.Product)
                  .WithMany() // Assuming Product does not need to track Reviews directly
                  .HasForeignKey(r => r.ProductId)
                  .OnDelete(DeleteBehavior.Cascade); // Optional: Cascade delete when Product is deleted

            entity.HasOne(r => r.Customer)
                  .WithMany() // Assuming Customer does not need to track Reviews directly
                  .HasForeignKey(r => r.CustomerId)
                  .OnDelete(DeleteBehavior.Cascade); // Optional: Cascade delete when Customer is deleted

            // Configure properties
            entity.Property(r => r.Rating)
                  .IsRequired(); // Ensure Rating is required

            entity.Property(r => r.Comment)
                  .HasMaxLength(500); // Optional: Set a maximum length for the comment

            entity.Property(r => r.DatePosted)
                  .IsRequired(); // Ensure DatePosted is required
        }
    }
}