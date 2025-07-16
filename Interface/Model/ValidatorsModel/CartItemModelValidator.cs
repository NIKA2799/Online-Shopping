using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Model.ValidatorsModel
{
    public class CartItemModelValidator : AbstractValidator<CartItemModel>
    {
        public CartItemModelValidator()
        {
            RuleFor(x => x.CartId)
                .GreaterThan(0)
                .WithMessage("A valid CartId is required.");

            RuleFor(x => x.ProductId)
                .GreaterThan(0)
                .WithMessage("A valid ProductId is required.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be at least 1.");

            // The Items collection should not be set on individual CartItemModel
            RuleFor(x => x.Items)
                .Null()
                .WithMessage("Nested Items should not be provided on CartItemModel.");
        }
    }
}
