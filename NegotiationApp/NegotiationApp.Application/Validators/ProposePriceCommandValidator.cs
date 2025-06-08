using FluentValidation;
using NegotiationApp.Application.Features.Negotiations.Commands;

namespace NegotiationApp.Application.Validators
{
    public class ProposePriceCommandValidator : AbstractValidator<ProposePriceCommand>
    {
        public ProposePriceCommandValidator()
        {
            RuleFor(x => x.NegotiationId)
                .NotEmpty().WithMessage(" negotiation ID cannot be empty.");
            RuleFor(x => x.ProposedPrice)
                .GreaterThan(0).WithMessage("proposed price has to be more than 0");
        }
    }
}