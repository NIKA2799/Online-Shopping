using Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Model
{
   public class OrderTrackingModel
    {
        public string OrderNumber { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public DateTime? DeliveredDate { get; set; }
    }
}
