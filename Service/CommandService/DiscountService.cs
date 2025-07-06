using Dto;
using Interface.Command;
using Interface.IRepositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.CommandService
{
    public class DiscountService : IDiscountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DiscountService> _logger;

        public DiscountService(
            IUnitOfWork unitOfWork,
            ILogger<DiscountService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Retrieves a discount entity by its code, or null if not found.
        /// </summary>
        public Discount GetByCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Discount code cannot be empty.", nameof(code));

#pragma warning disable CS8603 // Possible null reference return.
            return _unitOfWork.DiscountRepository
                .FindByCondition(expression: d => d.Code.Equals(code, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();
#pragma warning restore CS8603 // Possible null reference return.
        }

        /// <summary>
        /// Determines whether a discount code is valid (exists and not expired).
        /// </summary>
        public bool IsValid(string code)
        {
            try
            {
                var discount = GetByCode(code);
                bool valid = discount != null && discount.ExpirationDate > DateTime.UtcNow;
                _logger.LogInformation(
                    "Discount code {Code} validity: {Valid}", code, valid);
                return valid;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating discount code {Code}", code);
                return false;
            }
        }

        /// <summary>
        /// Applies the discount percentage to the total amount and returns the discounted total.
        /// </summary>
        public decimal ApplyDiscount(string code, decimal totalAmount)
        {
            var discount = GetByCode(code);
            if (discount == null || discount.ExpirationDate <= DateTime.UtcNow)
            {
                _logger.LogWarning(
                    "Discount code {Code} is invalid or expired", code);
                return totalAmount;
            }

            var discountAmount = totalAmount * (discount.DiscountPercentage / 100m);
            var finalAmount = totalAmount - discountAmount;
            _logger.LogInformation(
                "Applied discount {Code}: original {Total}, final {Final}",
                code, totalAmount, finalAmount);
            return finalAmount;
        }
    }
}
