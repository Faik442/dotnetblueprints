using FluentValidation;

namespace DotnetBlueprints.Auth.Application.Features.User.Commands.AssignRoleToUserInCompany;

public sealed class AssignRoleToUserInCompanyCommandValidator : AbstractValidator<AssignRoleToUserInCompanyCommand>
{
    public AssignRoleToUserInCompanyCommandValidator()
    {
        RuleFor(x => x.UserId).Must(id => id != Guid.Empty).WithMessage("User ID must be a valid GUID.");
        RuleFor(x => x.CompanyId).Must(id => id != Guid.Empty).WithMessage("Company ID must be a valid GUID.");
        RuleFor(x => x.RoleId).Must(id => id != Guid.Empty).WithMessage("Role ID must be a valid GUID.");
    }
}