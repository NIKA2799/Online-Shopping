using System;
using Interface.Command;
using Interface.Queries;
using Interface.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Webdemo.Models;

namespace Webdemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewCommandService _commandService;
        private readonly IReviewQureyService _queryService;

        public ReviewsController(
            IReviewCommandService commandService,
            IReviewQureyService queryService
        )
        {
            _commandService = commandService
                ?? throw new ArgumentNullException(nameof(commandService));
            _queryService = queryService
                ?? throw new ArgumentNullException(nameof(queryService));
        }

        /// <summary>
        /// Get all reviews for a given product.
        /// </summary>
        [HttpGet("product/{productId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetByProduct(int productId)
        {
            var reviews = _queryService.GetReviewsByProduct(productId);
            return Ok(reviews);
        }

        /// <summary>
        /// Get a single review by product and customer.
        /// </summary>
        [HttpGet("product/{productId}/user/{customerId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetByUser(int productId, int customerId)
        {
            var review = _queryService.GetReviewByUser(productId, customerId);
            if (review == null)
                return NotFound(new { message = "Review not found." });
            return Ok(review);
        }

        /// <summary>
        /// Create a new review.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Create([FromBody] ReviewModel model)
        {
            // Model is validated by FluentValidation / [ApiController]
            try
            {
                var id = _commandService.Insert(model);
                return CreatedAtAction(
                    nameof(GetByUser),
                    new { productId = model.ProductId, customerId = model.CustomerId },
                    new { id }
                );
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing review.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Update(int id, [FromBody] ReviewModel model)
        {
            // Model is validated by FluentValidation / [ApiController]
            _commandService.Update(id, model);
            return NoContent();
        }

        /// <summary>
        /// Delete a review.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult Delete(int id)
        {
            _commandService.Delete(id);
            return NoContent();
        }
    }
}
