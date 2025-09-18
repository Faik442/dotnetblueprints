using FluentValidation;

namespace DotnetBlueprints.Auth.Application.Features.User.Commands.AssignRoleToUserInCompany;

public sealed class AssignRoleToUserCommandValidator : AbstractValidator<AssignRoleToUserCommand>
{
    public AssignRoleToUserCommandValidator()
    {
        RuleFor(x => x.UserId).Must(id => id != Guid.Empty).WithMessage("User ID must be a valid GUID.");
        RuleForEach(x => x.RoleIds).NotEmpty().WithMessage("Each role ID must be a non-empty GUID.");
    }
}