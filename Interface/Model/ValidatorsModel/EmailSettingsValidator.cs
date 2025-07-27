using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Model.ValidatorsModel
{
    public class EmailSettingsValidator : AbstractValidator<EmailSettings>
    {
        public EmailSettingsValidator()
        {
            RuleFor(x => x.SmtpServer)
                .NotEmpty()
                .WithMessage("SMTP server address is required.");

            RuleFor(x => x.Port)
                .GreaterThan(0)
                .WithMessage("Port must be a positive integer.");

            RuleFor(x => x.SenderName)
                .NotEmpty()
                .WithMessage("Sender name is required.");

            RuleFor(x => x.SenderEmail)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("A valid sender email is required.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("SMTP password is required.");
        }
    }
}