using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBlueprints.Sales.Application.Offers.Commands.Update;

/// <summary>
/// Validates the <see cref="UpdateOfferCommand"/>.
/// </summary>
public class UpdateOfferCommandValidator : AbstractValidator<UpdateOfferCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateOfferCommandValidator"/> class.
    /// </summary>
    public UpdateOfferCommandValidator()
    {
        RuleFor(x => x.OfferId).NotEmpty();

        When(x => x.Title != null, () =>
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(100);
        });

        When(x => x.ValidUntil.HasValue, () =>
        {
            RuleFor(x => x.ValidUntil)
                .GreaterThan(DateTime.UtcNow);
        });

        When(x => x.Status.HasValue, () =>
        {
            RuleFor(x => x.Status)
                .IsInEnum();
        });

        RuleFor(x => x.UpdatedBy)
            .NotEmpty()
            .MaximumLength(100);
    }
}
