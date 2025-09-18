using FluentValidation;
using MediatR;

namespace DotnetBlueprints.Sales.Application.Features.Offers.Commands.Create;

/// <summary>
/// Validator for the <see cref="CreateOfferCommand"/> class.
/// Ensures that the offer title and validity date are provided and valid.
/// </summary>
public class CreateOfferCommandValidator : AbstractValidator<CreateOfferCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateOfferCommandValidator"/> class.
    /// </summary>
    public CreateOfferCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(100).WithMessage("Title must be at most 100 characters.");

        RuleFor(x => x.ValidUntil)
            .GreaterThan(DateTime.UtcNow).WithMessage("ValidUntil must be a future date.");
    }
}