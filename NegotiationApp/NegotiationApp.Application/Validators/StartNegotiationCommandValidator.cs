using FluentValidation;
using NegotiationApp.Application.Features.Negotiations.Commands;

namespace NegotiationApp.Application.Validators
{
    public class StartNegotiationCommandValidator : AbstractValidator<StartNegotiationCommand>
    {
        public StartNegotiationCommandValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("product ID cannot be empty.");
        }
    }
}