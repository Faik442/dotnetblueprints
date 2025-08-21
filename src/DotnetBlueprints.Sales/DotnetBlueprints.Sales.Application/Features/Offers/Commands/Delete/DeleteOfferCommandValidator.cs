using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBlueprints.Sales.Application.Features.Offers.Commands.Delete;

/// <summary>
/// Validates the <see cref="DeleteOfferCommand"/>.
/// </summary>
public class DeleteOfferCommandValidator : AbstractValidator<DeleteOfferCommand>
{
    public DeleteOfferCommandValidator()
    {
        RuleFor(x => x.OfferId)
            .NotEmpty().WithMessage("OfferId is required.");

        RuleFor(x => x.DeletedBy)
            .NotEmpty().WithMessage("DeletedBy is required.");
    }
}
