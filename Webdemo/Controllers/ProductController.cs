using System;
using System.Collections.Generic;
using Interface.Command;
using Interface.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Webdemo.Models;

namespace Webdemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductCommand _command;
        private readonly IProductQuery _query;

        public ProductsController(IProductCommand command, IProductQuery query)
        {
            _command = command ?? throw new ArgumentNullException(nameof(command));
            _query = query ?? throw new ArgumentNullException(nameof(query));
        }

        // ─── Queries ─────────────────────────────────────────────────

        // GET /api/products
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<ProductModel>> GetAll()
            => Ok(_query.FindAll());

        // GET /api/products/{id}
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ProductModel> GetById(int id)
        {
            var product = _query.Get(id);
            if (product == null)
                return NotFound(new { message = "Product not found." });

            return Ok(product);
        }

        // GET /api/products/search?keyword=foo
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<ProductModel>> Search([FromQuery] string keyword)
            => Ok(_query.SearchProducts(keyword));

        // GET /api/products/category/{categoryId}
        [HttpGet("category/{categoryId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<ProductModel>> ByCategory(int categoryId)
            => Ok(_query.GetProductsByCategory(categoryId));

        // GET /api/products/featured
        [HttpGet("featured")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<ProductModel>> Featured()
            => Ok(_query.GetFeaturedProducts());

        // GET /api/products/recent
        [HttpGet("recent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<ProductModel>> Recent()
            => Ok(_query.GetRecentlyAddedProducts());

        // GET /api/products/pricerange?min=10&max=50
        [HttpGet("pricerange")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<ProductModel>> ByPriceRange(
            [FromQuery] decimal min,
            [FromQuery] decimal max
        ) => Ok(_query.GetProductsByPriceRange(min, max));

        // GET /api/products/paged?pageNumber=1&pageSize=20
        [HttpGet("paged")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<ProductModel>> Paged(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10
        ) => Ok(_query.GetProducts(pageNumber, pageSize));

        // GET /api/products/related/{id}?take=4
        [HttpGet("related/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<ProductModel>> Related(int id, [FromQuery] int take = 4)
            => Ok(_query.GetRelatedProducts(id, take));


        // ─── Commands ────────────────────────────────────────────────

        // POST /api/products
        [HttpPost]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult Create([FromForm] ProductModel model)
        {
            var id = _command.Insert(model);
            return CreatedAtAction(nameof(GetById), new { id }, new { productId = id });
        }

        // PUT /api/products/{id}
        [HttpPut("{id:int}")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult Update(int id, [FromForm] ProductModel model)
        {
            if (_query.Get(id) == null)
                return NotFound(new { message = "Product not found." });

            _command.Update(id, model);
            return Ok(new { message = "Product updated." });
        }

        // DELETE /api/products/{id}
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult Delete(int id)
        {
            _command.Delete(id);
            return NoContent();
        }

        // PATCH /api/products/{id}/stock?newStock=42
        [HttpPatch("{id:int}/stock")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult UpdateStock(int id, [FromQuery] int newStock)
        {
            _command.UpdateStock(id, newStock);
            return Ok(new { message = "Stock updated.", productId = id, stock = newStock });
        }

        // PUT /api/products/{id}/toggle-availability
        [HttpPut("{id:int}/toggle-availability")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult ToggleAvailability(int id)
        {
            _command.ToggleAvailability(id);
            return Ok(new { message = "Availability toggled.", productId = id });
        }

        // PUT /api/products/{id}/toggle-featured
        [HttpPut("{id:int}/toggle-featured")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult ToggleFeatured(int id)
        {
            _command.ToggleFeatured(id);
            return Ok(new { message = "Featured toggled.", productId = id });
        }

        // GET /api/products/low-stock?threshold=5
        [HttpGet("low-stock")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<ProductModel>> LowStock([FromQuery] int threshold = 5)
            => Ok(_command.GetLowStockProducts(threshold));

        // POST /api/products/{id}/review
        [HttpPost("{id:int}/review")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult AddReview(int id, [FromBody] ReviewModel review)
        {
            if (id != review.ProductId)
                return BadRequest(new { message = "ProductId mismatch." });

            _command.AddReview(id, review);
            return StatusCode(StatusCodes.Status201Created, new { message = "Review added." });
        }
    }
}
