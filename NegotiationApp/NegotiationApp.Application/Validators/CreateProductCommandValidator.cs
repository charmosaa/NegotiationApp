using FluentValidation;
using NegotiationApp.Application.Features.Products.Commands;

namespace NegotiationApp.Application.Validators
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name can not be empty");
            RuleFor(x => x.BasePrice)
                .GreaterThan(0).WithMessage("Base price has to be greater than 0");
        }
    }
}