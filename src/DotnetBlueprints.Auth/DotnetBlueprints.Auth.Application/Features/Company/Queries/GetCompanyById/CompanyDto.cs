using AutoMapper.Execution;
using DotnetBlueprints.Auth.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBlueprints.Auth.Application.Features.Company.Queries.GetCompanyById;

public class CompanyDto
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
    public List<UserCompany>? Members { get; set; }

    /// <summary>
    /// Gets the collection of roles defined in the company scope.
    /// </summary>
    public List<Role>? Roles { get; set; }
}
