using DotnetBlueprints.Auth.Application.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBlueprints.Auth.Application.Features.Role.Commands.UpdateRole;

/// <summary>
/// Validates UpdateCompanyRoleCommand for basic field correctness and
/// company-scoped uniqueness of the role name.
/// </summary>
public sealed class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
{
    private readonly IAuthDbContext _context;

    public UpdateRoleCommandValidator(IAuthDbContext context)
    {
        _context = context;

        RuleFor(x => x.RoleId)
            .Must(id => id != Guid.Empty).WithMessage("Role ID must be a valid GUID.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MinimumLength(2).WithMessage("Name must be at least 2 characters.")
            .MaximumLength(64).WithMessage("Name must be at most 64 characters.")
            .Matches(@"^[A-Za-z0-9 ._\-]+$").WithMessage("Name contains invalid characters.")
            .Must(name => name.Trim() == name)
                .WithMessage("Name must not start or end with whitespace.");
    }
}