using FluentValidation;

namespace DotnetBlueprints.Auth.Application.Company.Commands.CreateCompany;

public class UpdateCompanyCommandValidator : AbstractValidator<CreateCompanyCommand>
{
    public UpdateCompanyCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Company name is required.")
            .MaximumLength(200).WithMessage("Company name must be at most 200 characters.");
    }
}
