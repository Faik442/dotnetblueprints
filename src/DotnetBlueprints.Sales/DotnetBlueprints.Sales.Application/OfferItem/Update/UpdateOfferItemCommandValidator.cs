using FluentValidation;

namespace DotnetBlueprints.Sales.Application.OfferItem.Update;

/// <summary>
/// Validator for <see cref="UpdateOfferItemCommand"/>.
/// </summary>
public class UpdateOfferItemCommandValidator : AbstractValidator<UpdateOfferItemCommand>
{
    public UpdateOfferItemCommandValidator()
    {
        RuleFor(x => x.OfferItemId)
            .NotEmpty().WithMessage("OfferItemId is required.");

        RuleFor(x => x.Name)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.Name));

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .When(x => x.Quantity.HasValue)
            .WithMessage("Quantity must be greater than zero.");

        RuleFor(x => x.UnitPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.UnitPrice.HasValue);
    }
}
