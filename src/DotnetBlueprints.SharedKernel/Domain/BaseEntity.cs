using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBlueprints.SharedKernel.Domain;

/// <summary>
/// Represents a generic base entity that supports customizable identifier types
/// such as int, Guid, long, or string. Includes standard audit properties.
/// </summary>
/// <typeparam name="T">The type of the entity's primary key.</typeparam>
public abstract class BaseEntity<T>
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    public T Id { get; set; } = default!;

    /// <summary>
    /// Gets or sets the timestamp when the entity was initially created.
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    /// <summary>
    /// Gets or sets the user or process that created the entity.
    /// </summary>
    public string CreatedBy { get; set; } = "System";

    /// <summary>
    /// Gets or sets the timestamp when the entity was last updated, if applicable.
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Gets or sets the user or process that last updated the entity, if applicable.
    /// </summary>
    public string? UpdatedBy { get; set; }
}

/// <summary>
/// Represents the default base entity with a <see cref=\"Guid\"/> identifier.
/// Automatically assigns a new GUID upon instantiation.
/// </summary>
public abstract class BaseEntity : BaseEntity<Guid>
{
    /// <summary>
    /// Initializes a new instance of the <see cref=\"BaseEntity\"/> class
    /// and generates a new unique identifier.
    /// </summary>
    public BaseEntity()
    {
        Id = Guid.NewGuid();
    }
}