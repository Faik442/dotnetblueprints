using FluentValidation;

namespace DotnetBlueprints.Sales.Application.Features.Offers.Queries.GetById;

/// <summary>
/// Validator for <see cref="GetOfferByIdQuery"/>.
/// </summary>
public class GetOfferByIdQueryValidator : AbstractValidator<GetOfferByIdQuery>
{
    public GetOfferByIdQueryValidator()
    {
        RuleFor(x => x.OfferId)
            .NotEmpty().WithMessage("OfferId must not be empty.");
    }
}
