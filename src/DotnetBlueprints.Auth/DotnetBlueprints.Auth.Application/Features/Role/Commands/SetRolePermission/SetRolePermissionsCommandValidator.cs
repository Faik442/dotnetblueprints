using FluentValidation;

namespace DotnetBlueprints.Auth.Application.Features.Role.Commands.SetRolePermission;

public sealed class SetRolePermissionsCommandValidator : AbstractValidator<SetRolePermissionsCommand>
{
    public SetRolePermissionsCommandValidator()
    {
        RuleFor(x => x.RoleId).Must(id => id != Guid.Empty).WithMessage("Role ID must be a valid GUID.");

        RuleFor(x => x.PermissionIds).NotNull().WithMessage("PermissionIds must be valid Guids");
    }
}
