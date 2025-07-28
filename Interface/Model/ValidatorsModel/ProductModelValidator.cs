using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webdemo.Models;

namespace Interface.Model.ValidatorsModel
{
    public class ProductModelValidator : AbstractValidator<ProductModel>
    {
        public ProductModelValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .When(x => x.Id != default)
                .WithMessage("A valid Id (> 0) is required when updating.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Product name is required.");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Product description is required.");

            RuleFor(x => x.Price)
                .GreaterThan(0)
                .WithMessage("Price must be greater than zero.");

            RuleFor(x => x.Stock)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Stock cannot be negative.");

            RuleFor(x => x.ImagePath)
                .NotEmpty()
                .WithMessage("ImagePath is required.");

            RuleFor(x => x.ImageFile)
                .NotNull()
                .WithMessage("ImageFile is required.")
                .DependentRules(() =>
                {
                    RuleFor(x => x.ImageFile.Length)
                        .GreaterThan(0)
                        .WithMessage("ImageFile cannot be empty.");
                });

            // Optional flags
            RuleFor(x => x.IsFeatured)
                .NotNull();

            RuleFor(x => x.CreateDate)
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("CreateDate cannot be in the future.");
        }
    }
}
