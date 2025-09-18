using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBlueprints.Auth.Application.Features.User.Commands.RemoveRoleFromUserInCompany;

public sealed class RemoveRoleFromUserCommandValidator : AbstractValidator<RemoveRoleFromUserCommand>
{
    public RemoveRoleFromUserCommandValidator()
    {
        RuleFor(x => x.UserId).Must(id => id != Guid.Empty).WithMessage("User ID must be a valid GUID.");
        RuleFor(x => x.RoleIds).NotNull().WithMessage("RoleIds must be valid GUIDs");
    }
}
