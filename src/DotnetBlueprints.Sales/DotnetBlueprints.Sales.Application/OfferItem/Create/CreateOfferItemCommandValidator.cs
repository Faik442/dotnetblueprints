using FluentValidation;

namespace DotnetBlueprints.Sales.Application.OfferItem.Create;

/// <summary>
/// Validator for <see cref="CreateOfferItemCommand"/> to ensure all required properties are valid.
/// </summary>
public class CreateOfferItemCommandValidator : AbstractValidator<CreateOfferItemCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateOfferItemCommandValidator"/> class.
    /// </summary>
    public CreateOfferItemCommandValidator()
    {
        RuleFor(x => x.OfferId)
            .NotEmpty().WithMessage("OfferId is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(250).WithMessage("Name must not exceed 250 characters.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.");

        RuleFor(x => x.UnitPrice)
            .GreaterThanOrEqualTo(0).WithMessage("UnitPrice must be zero or greater.");
    }
}
