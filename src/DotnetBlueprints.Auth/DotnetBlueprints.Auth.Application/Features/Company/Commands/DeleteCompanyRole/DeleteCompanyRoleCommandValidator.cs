using FluentValidation;
using System;

namespace DotnetBlueprints.Auth.Application.Features.Company.Commands.DeleteCompanyRole;

/// <summary>
/// Validates DeleteCompanyRoleCommand ensuring identifiers are provided.
/// </summary>
public sealed class DeleteCompanyRoleCommandValidator : AbstractValidator<DeleteCompanyRoleCommand>
{
    public DeleteCompanyRoleCommandValidator()
    {
        RuleFor(x => x.CompanyId)
            .Must(id => id != Guid.Empty).WithMessage("Company ID must be a valid GUID.");

        RuleFor(x => x.RoleId)
            .Must(id => id != Guid.Empty).WithMessage("Role ID must be a valid GUID.");
    }
}
