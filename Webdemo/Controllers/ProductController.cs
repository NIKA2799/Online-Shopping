using Interface.Command;
using Interface.Queries;
using Microsoft.AspNetCore.Mvc;
using Webdemo.Models;

namespace Webdemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductCommand _productCommand;
        private readonly IProductQuery _productQuery;

        public ProductController(IProductCommand productCommand, IProductQuery productQuery)
        {
            _productCommand = productCommand;
            _productQuery = productQuery;
        }

        // 🔍 Get all products
        [HttpGet]
        public IActionResult GetAll()
        {
            var products = _productQuery.FindAll();
            return Ok(products);
        }

        // 🔍 Get product by ID
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var product = _productQuery.Get(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        // 🔍 Search
        [HttpGet("search")]
        public IActionResult Search(string keyword)
        {
            var result = _productQuery.SearchProducts(keyword);
            return Ok(result);
        }

        // 🔍 Featured Products
        [HttpGet("featured")]
        public IActionResult GetFeatured()
        {
            var featured = _productQuery.GetFeaturedProducts();
            return Ok(featured);
        }

        // 🔍 Related Products
        [HttpGet("related/{productId}")]
        public IActionResult GetRelated(int productId)
        {
            var related = _productQuery.GetRelatedProducts(productId);
            return Ok(related);
        }

        // ➕ Add new product
        [HttpPost]
        public IActionResult Create([FromForm] ProductModel model)
        {
            var id = _productCommand.Insert(model);
            return Ok(new { Message = "Product created", ProductId = id });
        }

        // ✏️ Update product
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromForm] ProductModel model)
        {
            _productCommand.Update(id, model);
            return Ok(new { Message = "Product updated" });
        }

        // ❌ Delete product
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _productCommand.Delete(id);
            return Ok(new { Message = "Product deleted" });
        }

        // 🌟 Toggle featured status
        [HttpPut("{id}/toggle-featured")]
        public IActionResult ToggleFeatured(int id)
        {
            _productCommand.ToggleFeatured(id);
            return Ok(new { Message = "Featured status toggled" });
        }

        // 📦 Toggle availability
        [HttpPut("{id}/toggle-availability")]
        public IActionResult ToggleAvailability(int id)
        {
            _productCommand.ToggleAvailability(id);
            return Ok(new { Message = "Availability toggled" });
        }
    }
}