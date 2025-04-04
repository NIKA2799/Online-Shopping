using Dto;
using Interface.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Command
{
    public interface IOrderCommandService : ICommandModel<OrderModel>
    {
         void CancelOrder(int orderId);
         void UpdateOrderStatus(int orderId, OrderStatus status);
         int Checkout(CheckoutModel checkoutModel);
         OrderStatus? TrackOrderStatus(int orderId, int userId);
    }
}
