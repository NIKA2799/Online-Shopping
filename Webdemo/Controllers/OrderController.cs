using Dto;
using Interface.Command;
using Interface.Model;
using Interface.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Webdemo.Controllers
{
        [Route("api/[controller]")]
        [ApiController]
        public class OrderController : ControllerBase
        {
            private readonly IOrderCommandService _orderCommandService;
            private readonly IOrderQurey _orderQueryService;

            public OrderController(IOrderCommandService orderCommandService, IOrderQurey orderQueryService)
            {
                _orderCommandService = orderCommandService;
                _orderQueryService = orderQueryService;
            }

            [HttpPost("checkout")]
            public IActionResult Checkout([FromBody] CheckoutModel checkoutModel)
            {
                try
                {
                    var orderId = _orderCommandService.Checkout(checkoutModel);
                    return Ok(new { Message = "Order placed successfully", OrderId = orderId });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { Message = ex.Message });
                }
            }

            [HttpPost("cancel/{id}")]
            public IActionResult CancelOrder(int id)
            {
                _orderCommandService.CancelOrder(id);
                return Ok(new { Message = "Order cancelled successfully" });
            }

            [HttpGet("all")]
            public IActionResult GetAllOrders()
            {
                var orders = _orderQueryService.FindAll();
                return Ok(orders);
            }

            [HttpGet("user/{customerId}")]
            public IActionResult GetOrdersByUser(int customerId)
            {
                var orders = _orderQueryService.GetOrdersByUser(customerId);
                return Ok(orders);
            }

            [HttpGet("{id}")]
            public IActionResult GetOrderById(int id)
            {
                var order = _orderQueryService.Get(id);
                if (order == null) return NotFound(new { Message = "Order not found" });
                return Ok(order);
            }

            [HttpPut("update/{id}")]
            public IActionResult UpdateOrder(int id, [FromBody] OrderModel orderModel)
            {
                _orderCommandService.Update(id, orderModel);
                return Ok(new { Message = "Order updated successfully" });
            }

            [HttpPut("update-status/{orderId}")]
            public IActionResult UpdateOrderStatus(int orderId, [FromBody] OrderStatus status)
            {
                _orderCommandService.UpdateOrderStatus(orderId, status);
                return Ok(new { Message = "Order status updated successfully" });
            }

            [HttpDelete("delete/{id}")]
            public IActionResult DeleteOrder(int id)
            {
                _orderCommandService.Delete(id);
                return Ok(new { Message = "Order deleted successfully" });
            }
        }
    }

