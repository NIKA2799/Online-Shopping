using Interface.Command;
using Interface.Model;
using Microsoft.AspNetCore.Mvc;

namespace Webdemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICarteService _cartService;

        public CartController(ICarteService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet("user/{customerId}")]
        public IActionResult GetCartByUserId(int customerId)
        {
            var cart = _cartService.GetCartByUserId(customerId);
            if (cart == null) return NotFound(new { Message = "Cart not found" });
            return Ok(cart);
        }

        [HttpPost("add")]
        public IActionResult AddItemToCart(int customerId, int productId, int quantity)
        {
            var result = _cartService.AddItemToCart(customerId, productId, quantity);
            if (!result) return BadRequest(new { Message = "Failed to add item to cart" });
            return Ok(new { Message = "Item added successfully" });
        }

        [HttpDelete("remove/{customerId}/{cartItemId}")]
        public IActionResult RemoveItemFromCart(int customerId, int cartItemId)
        {
            var result = _cartService.RemoveItemFromCart(customerId, cartItemId);
            if (!result) return NotFound(new { Message = "Item not found in cart" });
            return Ok(new { Message = "Item removed successfully" });
        }

        [HttpPut("update/{customerId}/{cartItemId}")]
        public IActionResult UpdateItemQuantity(int customerId, int cartItemId, int quantity)
        {
            var result = _cartService.UpdateItemQuantity(customerId, cartItemId, quantity);
            if (!result) return NotFound(new { Message = "Cart item not found" });
            return Ok(new { Message = "Quantity updated successfully" });
        }

        [HttpDelete("clear/{customerId}")]
        public IActionResult ClearCart(int customerId)
        {
            var result = _cartService.ClearCart(customerId);
            if (!result) return NotFound(new { Message = "Cart not found" });
            return Ok(new { Message = "Cart cleared successfully" });
        }

        [HttpPost("checkout/{userId}")]
        public IActionResult Checkout(int userId, [FromBody] CheckoutModel model)
        {
            var result = _cartService.Checkout(userId, model);
            if (!result) return BadRequest(new { Message = "Checkout failed" });
            return Ok(new { Message = "Checkout successful" });
        }

        [HttpGet("items/{cartId}")]
        public IActionResult GetCartItems(int cartId)
        {
            var items = _cartService.GetCartItems(cartId);
            return Ok(items);
        }
        public IActionResult DeletCart(int id)
        {
            _cartService.DeletCart(id);
            return Ok(new { Message = "Cart delete successfully" });
        }

    }
}