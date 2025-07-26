using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Model.ValidatorsModel
{

    public class CategoryModelValidator : AbstractValidator<CategoryModel>
    {
        public CategoryModelValidator()
        {
            // Id is only required on Update scenarios
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .When(x => x.Id != default)
                .WithMessage("A valid Id (> 0) is required when updating.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Description is required.");

            // CreateDate should either be default (for inserts)
            // or not in the future
            RuleFor(x => x.CreateDate)
                .Must(dt => dt == default || dt <= DateTime.UtcNow)
                .WithMessage("CreateDate must be set by the server or be in the past.");
        }
    }
}