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
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Timestamp)
                   .HasDefaultValueSql("GETUTCDATE()");
            builder.Property(a => a.Action)
                   .IsRequired()
                   .HasMaxLength(500);
            builder.Property(a => a.EntityName)
                   .HasMaxLength(128);
            builder.Property(a => a.EntityId)
                   .HasMaxLength(64);
        }
    }
}