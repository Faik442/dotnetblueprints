using DotnetBlueprints.Auth.Application.Features.Company.Queries.GetCompanyById;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBlueprints.Auth.Application.Features.Company.Queries.GetCompanies;

public class GetCompaniesQueryValidator : AbstractValidator<GetCompaniesQuery>
{
    public GetCompaniesQueryValidator()
    {
        RuleFor(x => x.pageSize)
            .Must(x => x <= 100).WithMessage("Max page size must be equal or less than 100.");
    }
}