using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBlueprints.Sales.Application.Features.OfferItem.Delete;

using FluentValidation;

public class DeleteOfferItemCommandValidator : AbstractValidator<DeleteOfferItemCommand>
{
    public DeleteOfferItemCommandValidator()
    {
        RuleFor(x => x.OfferItemId)
            .NotEmpty().WithMessage("Offer item id is required.");

        RuleFor(x => x.DeletedBy)
            .NotEmpty().WithMessage("DeletedBy is required.")
            .MaximumLength(100).WithMessage("DeletedBy cannot be longer than 100 characters.");
    }
}

