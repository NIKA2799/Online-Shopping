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
            // Primary Key
            entity.HasKey(r => r.Id);

            // Rating
            entity.Property(r => r.Rating).IsRequired();

            // Comment
            entity.Property(r => r.Comment)
                  .HasMaxLength(1000);

            // DatePosted
            entity.Property(r => r.DatePosted)
                  .HasDefaultValueSql("GETUTCDATE()");

            // Product (1 Product → Many Reviews)
            entity.HasOne(r => r.Product)
                  .WithMany()
                  .HasForeignKey(r => r.ProductId)
                  .OnDelete(DeleteBehavior.Restrict); // ✅ შეცვლილია Restrict-ად

            // Customer (1 Customer → Many Reviews)
            entity.HasOne(r => r.Customer)
                  .WithMany()
                  .HasForeignKey(r => r.CustomerId)
                  .OnDelete(DeleteBehavior.Cascade); // ✅ კასკადი მომხმარებელზე დარჩება

            // Table name
            entity.ToTable("Reviews");
        }
    }
}