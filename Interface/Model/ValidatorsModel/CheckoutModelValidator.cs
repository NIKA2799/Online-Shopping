using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Model.ValidatorsModel
{
    public class CheckoutModelValidator : AbstractValidator<CheckoutModel>
    {
        public CheckoutModelValidator()
        {
            // Ensure CartId and UserId are provided
            RuleFor(x => x.CartId)
                .NotEmpty()
                .WithMessage("CartId is required.");

            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("UserId is required.");

            // Addresses and payment method
            RuleFor(x => x.ShippingAddress)
                .NotEmpty()
                .WithMessage("Shipping address is required.");

            RuleFor(x => x.BillingAddress)
                .NotEmpty()
                .WithMessage("Billing address is required.");

            RuleFor(x => x.PaymentMethod)
                .NotEmpty()
                .WithMessage("Payment method is required.");

            // Credit‑card rules only if PaymentMethod == "CreditCard"
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            When(x => x.PaymentMethod.Equals("CreditCard", StringComparison.OrdinalIgnoreCase), () =>
            {
                RuleFor(x => x.CreditCardNumber)
                    .CreditCard()
                    .WithMessage("A valid credit card number is required.");

                RuleFor(x => x.CreditCardExpiration)
                    .Matches(@"^(0[1-9]|1[0-2])\/?([0-9]{2})$")
                    .WithMessage("Credit card expiration must be in MM/YY format.");

                RuleFor(x => x.CreditCardCVC)
                    .Matches(@"^\d{3,4}$")
                    .WithMessage("CVC must be a 3‐ or 4‐digit number.");
            });
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            // Order total must be positive
            RuleFor(x => x.OrderTotal)
                .GreaterThan(0)
                .WithMessage("Order total must be greater than zero.");

            // CartItems must be present and each item valid
            RuleFor(x => x.CartItems)
                .NotEmpty()
                .WithMessage("Cart must contain at least one item.")
                .ForEach(item =>
                {
                    item.SetValidator(new CartItemModelValidator());
                });

           
            RuleFor(x => x.OrderDate)
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("Order date cannot be in the future.");
        }
    }
}