using FluentValidation;

namespace DotnetBlueprints.Auth.Application.Features.User.Commands;

public sealed class AddUserToCompanyCommandValidator : AbstractValidator<AddUserToCompanyCommand>
{
    public AddUserToCompanyCommandValidator()
    {
        RuleFor(x => x.UserId).Must(id => id != Guid.Empty).WithMessage("User ID must be a valid GUID.");
        RuleFor(x => x.CompanyId).Must(id => id != Guid.Empty).WithMessage("Company ID must be a valid GUID.");
        RuleForEach(x => x.RoleIds ?? Enumerable.Empty<Guid>()).NotEmpty();
    }
}