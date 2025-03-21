using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto
{
    public class Payment :IEntity 
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public string? PaymentMethod { get; set; } // e.g., Credit Card, PayPal
        public bool IsPaid { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
