using DotnetBlueprints.SharedKernel.Exceptions;
using FluentValidation;

namespace DotnetBlueprints.Sales.Application.Features.Offers.Commands.UpdateStatus;

/// <summary>
/// Validates the <see cref="UpdateOfferStatusCommand"/> for business rules and data integrity.
/// </summary>
public class UpdateOfferStatusCommandValidator : AbstractValidator<UpdateOfferStatusCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateOfferStatusCommandValidator"/> class.
    /// Sets up validation rules for required fields and valid values.
    /// </summary>
    public UpdateOfferStatusCommandValidator()
    {
        RuleFor(x => x.OfferId)
            .NotEmpty()
            .WithMessage(ExceptionMessages.FieldRequired("OfferId"));

        RuleFor(x => x.NewStatus)
            .IsInEnum()
            .WithMessage(ExceptionMessages.InvalidEnumValue(nameof(UpdateOfferStatusCommand.NewStatus)));

        RuleFor(x => x.ChangedBy)
            .NotEmpty()
            .WithMessage(ExceptionMessages.FieldRequired("ChangedBy"));
    }
}
