using DotnetBlueprints.Auth.Application.Mappings;

namespace DotnetBlueprints.Auth.Application.Features.Company.Queries.GetCompanyById;

public class CompanyDto : IMapFrom<Domain.Entities.Company>
{
    /// <summary>
    /// Gets the id of the company.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets the name of the company.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets the collection of company members.
    /// </summary>
    public List<Domain.Entities.User>? Members { get; set; }

    /// <summary>
    /// Gets the collection of roles defined in the company scope.
    /// </summary>
    public List<Domain.Entities.Role>? Roles { get; set; }
}
