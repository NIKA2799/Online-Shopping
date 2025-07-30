using System;
using Interface.Command;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Service.CommandService; // or wherever DiscountService lives

namespace Webdemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiscountController : ControllerBase
    {
        private readonly IDiscountService _discountService;
        private readonly ILogger<DiscountController> _logger;

        public DiscountController(
            IDiscountService discountService,
            ILogger<DiscountController> logger
        )
        {
            _discountService = discountService
                ?? throw new ArgumentNullException(nameof(discountService));
            _logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Retrieve discount details by code.
        /// </summary>
        [HttpGet("{code}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetByCode(string code)
        {
            var disc = _discountService.GetByCode(code);
            if (disc == null)
                return NotFound(new { message = $"Discount '{code}' not found." });

            return Ok(disc);
        }

        /// <summary>
        /// Check if a discount code is valid (exists and not expired).
        /// </summary>
        [HttpGet("{code}/validate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult IsValid(string code)
        {
            var valid = _discountService.IsValid(code);
            return Ok(new { code, valid });
        }

        /// <summary>
        /// Apply the discount to a total amount.
        /// </summary>
        [HttpGet("{code}/apply")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Apply(string code, [FromQuery] decimal totalAmount)
        {
            if (totalAmount < 0)
                return BadRequest(new { message = "totalAmount must be non‐negative." });

            var discounted = _discountService.ApplyDiscount(code, totalAmount);
            return Ok(new
            {
                code,
                originalAmount = totalAmount,
                discountedAmount = discounted
            });
        }
    }
}
