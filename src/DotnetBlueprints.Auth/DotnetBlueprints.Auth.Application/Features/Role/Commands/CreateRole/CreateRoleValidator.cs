using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBlueprints.Auth.Application.Features.Role.Commands.CreateRole;

public class CreateRoleToCompanyCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleToCompanyCommandValidator()
    {
        RuleFor(x => x.RoleName)
            .NotEmpty().WithMessage("Role name is required.")
            .MaximumLength(100).WithMessage("Role name must be at most 100 characters.");

        RuleFor(x => x.Permissions)
            .NotNull().WithMessage("Permissions cannot be null.")
            .Must(p => p.Any()).WithMessage("At least one permission must be provided.");
    }
}
