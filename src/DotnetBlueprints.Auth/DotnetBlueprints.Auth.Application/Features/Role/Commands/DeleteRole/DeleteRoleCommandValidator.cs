using FluentValidation;
using System;

namespace DotnetBlueprints.Auth.Application.Features.Role.Commands.DeleteRole;

/// <summary>
/// Validates DeleteCompanyRoleCommand ensuring identifiers are provided.
/// </summary>
public sealed class DeleteRoleCommandValidator : AbstractValidator<DeleteRoleCommand>
{
    public DeleteRoleCommandValidator()
    {
        RuleFor(x => x.RoleId)
            .Must(id => id != Guid.Empty).WithMessage("Role ID must be a valid GUID.");
    }
}
