using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Model.ValidatorsModel
{
    public class OrderTrackingModelValidator : AbstractValidator<OrderTrackingModel>
    {
        public OrderTrackingModelValidator()
        {
            RuleFor(x => x.OrderNumber)
                .NotEmpty()
                .WithMessage("Order number is required.");

            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("Order Status is invalid.");

            RuleFor(x => x.OrderDate)
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("Order date cannot be in the future.");

            RuleFor(x => x.ShippedDate)
                .Must((model, sd) => sd == null || sd >= model.OrderDate)
                .WithMessage("Shipped date cannot be before the order date.");

            RuleFor(x => x.DeliveredDate)
                .Must((model, dd) =>
                    dd == null
                    || (model.ShippedDate.HasValue && dd >= model.ShippedDate.Value))
                .WithMessage("Delivered date cannot be before the shipped date.");
        }
    }
}