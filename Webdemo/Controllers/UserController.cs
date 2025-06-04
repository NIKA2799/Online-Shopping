using Interface.Model;
using Microsoft.AspNetCore.Mvc;
using Service.CommandService;
using Webdemo.Models;

namespace Webdemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{id}")]
        public ActionResult<UserModel> GetCustomerById(int id)
        {
            var user = _userService.GetCustomerById(id);
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        [HttpGet]
        public ActionResult<IEnumerable<UserModel>> GetAllCustomers()
        {
            var users = _userService.GetAllCustomers();
            return Ok(users);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateCustomer(int id, [FromBody] UserModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _userService.UpdateCustomer(id, model);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCustomer(int id)
        {
            _userService.DeleteCustomer(id);
            return NoContent();
        }

        [HttpPost("{customerId}/products")]
        public ActionResult<int> UploadProduct(int customerId, [FromForm] ProductModel productModel)
        {
            var productId = _userService.UploadProduct(productModel, customerId);
            return Ok(productId);
        }

        [HttpGet("{customerId}/products")]
        public ActionResult<IEnumerable<ProductModel>> GetMyProducts(int customerId)
        {
            var products = _userService.GetMyProducts(customerId);
            return Ok(products);
        }
    }
}
