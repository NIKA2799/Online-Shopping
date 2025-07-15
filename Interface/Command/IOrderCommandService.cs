using Dto;
using Interface.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Command
{
    public interface IOrderCommandService 
    {
        /// <summary>
        /// Creates a new order from the given model.
        /// </summary>
        /// <returns>The newly created Order Id.</returns>
        int Insert(OrderModel model);

        /// <summary>
        /// Updates an existing order.
        /// </summary>
        /// <returns>True if the update succeeded.</returns>
        bool Update(int id, OrderModel model);

        /// <summary>
        /// Deletes an order.
        /// </summary>
        /// <returns>True if the delete succeeded.</returns>
        bool Delete(int id);

        /// <summary>
        /// Changes the status of an order.
        /// </summary>
        /// <returns>True if the status update succeeded.</returns>
        bool UpdateOrderStatus(int orderId, OrderStatus newStatus);

        /// <summary>
        /// Cancels an order (only if it belongs to the given customer).
        /// </summary>
        /// <returns>True if the cancellation succeeded.</returns>
        bool CancelOrder(int orderId, int customerId);

        /// <summary>
        /// Performs checkout for a customer’s cart, creates the order, details, updates stock,
        /// applies any discount code, and clears the cart.
        /// </summary>
        /// <returns>The newly created Order Id.</returns>
        int Checkout(CheckoutModel model);

        /// <summary>
        /// Returns the current status of an order for a given customer (or null if not found/unauthorized).
        /// </summary>
        OrderStatus? TrackOrderStatus(int orderId, int customerId);
    }
}
