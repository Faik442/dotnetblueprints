using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBlueprints.Auth.Application.Features.User.Commands.RemoveUserFromCompany;

public sealed class RemoveUserFromCompanyCommandValidator : AbstractValidator<RemoveUserFromCompanyCommand>
{
    public RemoveUserFromCompanyCommandValidator()
    {
        RuleFor(x => x.UserId).Must(id => id != Guid.Empty).WithMessage("User ID must be a valid GUID.");
        RuleFor(x => x.CompanyId).Must(id => id != Guid.Empty).WithMessage("Company ID must be a valid GUID.");
    }
}
