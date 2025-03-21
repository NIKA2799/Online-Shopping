using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto
{
    public class Discount : IEntity
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public decimal DiscountPercentage { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
