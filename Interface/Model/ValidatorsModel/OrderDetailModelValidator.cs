using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Model.ValidatorsModel
{
    public class OrderDetailModelValidator : AbstractValidator<OrderDetailModel>
    {
        public OrderDetailModelValidator()
        {
            // Id only required on updates
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .When(x => x.Id != default)
                .WithMessage("A valid Id (> 0) is required when updating an order detail.");

            // Foreign keys
            RuleFor(x => x.OrderId)
                .GreaterThan(0)
                .WithMessage("OrderId must be greater than zero.");

            RuleFor(x => x.ProductId)
                .GreaterThan(0)
                .WithMessage("ProductId must be greater than zero.");

            // Quantity
            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be at least 1.");

            // Unit price
            RuleFor(x => x.UnitPrice)
                .GreaterThanOrEqualTo(0)
                .WithMessage("UnitPrice cannot be negative.");
        }
    }
}