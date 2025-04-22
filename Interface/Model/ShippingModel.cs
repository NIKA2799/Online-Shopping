
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Model
{
   public class ShippingModel: IEntityModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingMethod { get; set; }
        public DateTime? ShippedDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
    }
}
