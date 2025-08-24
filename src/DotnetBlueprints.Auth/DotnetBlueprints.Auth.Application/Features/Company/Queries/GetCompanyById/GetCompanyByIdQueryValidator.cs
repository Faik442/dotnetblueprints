using FluentValidation;

namespace DotnetBlueprints.Auth.Application.Features.Company.Queries.GetCompanyById;

public class GetCompanyByIdQueryValidator : AbstractValidator<GetCompanyByIdQuery>
{
    public GetCompanyByIdQueryValidator()
    {
        RuleFor(x => x.CompanyId)
            .Must(id => id != Guid.Empty).WithMessage("Company ID must be a valid GUID.");
    }
}
