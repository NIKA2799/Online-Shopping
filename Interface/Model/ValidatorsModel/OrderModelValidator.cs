using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Model.ValidatorsModel
{
    public class OrderModelValidator : AbstractValidator<OrderModel>
    {
        public OrderModelValidator()
        {
            // Id only required on update scenarios
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .When(x => x.Id != default)
                .WithMessage("A valid Id (> 0) is required when updating an order.");

            // Foreign key
            RuleFor(x => x.CustomerId)
                .GreaterThan(0)
                .WithMessage("CustomerId must be greater than zero.");

            // Dates
            RuleFor(x => x.OrderDate)
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("Order date cannot be in the future.");

            // Amount
            RuleFor(x => x.TotalAmount)
                .GreaterThanOrEqualTo(0)
                .WithMessage("TotalAmount cannot be negative.");

            // Addresses and payment
            RuleFor(x => x.ShippingAddress)
                .NotEmpty()
                .WithMessage("ShippingAddress is required.");

            RuleFor(x => x.BillingAddress)
                .NotEmpty()
                .WithMessage("BillingAddress is required.");

            RuleFor(x => x.PaymentMethod)
                .NotEmpty()
                .WithMessage("PaymentMethod is required.");
        }
    }
}