using DotnetBlueprints.Auth.Domain.Enums;
using DotnetBlueprints.SharedKernel.Abstractions;
using DotnetBlueprints.SharedKernel.Domain;
using System.Security;

namespace DotnetBlueprints.Auth.Domain.Entities;

/// <summary>
/// Represents a company as an aggregate root in the Auth context.
/// Audit fields (CreatedBy, CreatedDate, UpdatedBy, UpdatedDate) 
/// are inherited from BaseEntity and populated automatically by EF interceptors.
/// </summary>
public sealed class Company : BaseEntity, IAggregateRoot
{
    private readonly List<User> _members = new();

    private Company() { }

    public Company(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Company name cannot be empty.", nameof(name));

        Name = name;
    }

    /// <summary>
    /// Gets the name of the company.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the collection of company members.
    /// </summary>
    public IReadOnlyCollection<User> Members => _members;

    /// <summary>
    /// Factory method for creating a new company instance.
    /// </summary>
    public static Company Create(string name) => new(name);

    /// <summary>
    /// Renames the company.
    /// </summary>
    public void Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Company name cannot be empty.", nameof(newName));

        if (Name.Equals(newName, StringComparison.OrdinalIgnoreCase))
            return;

        Name = newName;
    }

    /// <summary>
    /// Marks the company as soft deleted.
    /// </summary>
    public void Delete() => IsDeleted = true;
}
