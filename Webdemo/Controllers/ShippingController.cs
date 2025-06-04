using Interface.Command;
using Interface.Model;
using Microsoft.AspNetCore.Mvc;

namespace Webdemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShippingController : ControllerBase
    {
        private readonly IShippingService _shippingService;

        public ShippingController(IShippingService shippingService)
        {
            _shippingService = shippingService;
        }

        // GET: api/Shipping/{id}
        [HttpGet("{id}")]
        public ActionResult<ShippingModel> GetShippingById(int id)
        {
            var shipping = _shippingService.GetShippingById(id);
            if (shipping == null)
                return NotFound();
            return Ok(shipping);
        }

        // GET: api/Shipping
        [HttpGet]
        public ActionResult<IEnumerable<ShippingModel>> GetAllShippings()
        {
            var shippings = _shippingService.GetAllShippings();
            return Ok(shippings);
        }

        // POST: api/Shipping
        [HttpPost]
        public ActionResult<int> CreateShipping([FromBody] ShippingModel shippingModel)
        {
            if (shippingModel == null)
                return BadRequest();

            var id = _shippingService.CreateShipping(shippingModel);
            return CreatedAtAction(nameof(GetShippingById), new { id }, id);
        }

        // PUT: api/Shipping/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateShipping(int id, [FromBody] ShippingModel shippingModel)
        {
            if (shippingModel == null || shippingModel.Id != id)
                return BadRequest();

            var updated = _shippingService.UpdateShipping(shippingModel);
            if (!updated)
                return NotFound();

            return NoContent();
        }

        // DELETE: api/Shipping/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteShipping(int id)
        {
            var deleted = _shippingService.DeleteShipping(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
