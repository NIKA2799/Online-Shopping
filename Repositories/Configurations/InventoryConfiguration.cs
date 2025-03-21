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
    public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
    {
        public void Configure(EntityTypeBuilder<Inventory> entity)
        {
            // Configure the primary key
            entity.HasKey(i => i.Id);

            // Configure the foreign key relationship with Product
            entity.HasOne(i => i.Product)
                  .WithMany() // Assuming Product doesn't need to track Inventory directly
                  .HasForeignKey(i => i.ProductId)
                  .OnDelete(DeleteBehavior.Cascade); // Optional: Cascade delete when Product is deleted

            // Configure properties
            entity.Property(i => i.QuantityAvailable)
                  .IsRequired(); // Optional: Make QuantityAvailable required

            entity.Property(i => i.LastUpdated)
                  .IsRequired() // Optional: Make LastUpdated required
                  .HasDefaultValueSql("GETDATE()"); // Optional: Default to current date/time
        }
    }
}