using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto
{
    public class AuditLog : IEntity
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public string UserId { get; set; }            // optional: who performed the action
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public string? Action { get; set; }            // e.g. “Order 123 cancelled”
        public string? EntityName { get; set; }        // e.g. “Order”
        public string? EntityId { get; set; }          // e.g. “123”
    }
}