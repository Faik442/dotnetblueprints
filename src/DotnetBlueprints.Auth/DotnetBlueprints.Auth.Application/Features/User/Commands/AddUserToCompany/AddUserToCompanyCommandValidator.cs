using FluentValidation;

namespace DotnetBlueprints.Auth.Application.Features.User.Commands.AddUserToCompany;

public sealed class AddUserToCompanyCommandValidator : AbstractValidator<AddUserToCompanyCommand>
{
    public AddUserToCompanyCommandValidator()
    {
        RuleFor(x => x.CompanyId).Must(id => id != Guid.Empty).WithMessage("Company ID must be a valid GUID.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email must not be empty.")
            .EmailAddress().WithMessage("Invalid email format.")
            .MaximumLength(256).WithMessage("Email must not exceed 256 characters.");

        RuleFor(x => x.DisplayName)
            .NotEmpty().WithMessage("DisplayName must not be empty.")
            .MaximumLength(100).WithMessage("DisplayName must not exceed 100 characters.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password must not be empty.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
            .MaximumLength(25).WithMessage("Password must not exceed 25 characters.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.")
            .Matches(@"[\p{P}\p{S}]").WithMessage("Password must contain at least one punctuation or special character.");

        RuleForEach(x => x.RoleIds)
            .NotEmpty().WithMessage("RoleId must not be empty.");
    }
}