using DotnetBlueprints.SharedKernel.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBlueprints.SharedKernel.Audit;

/// <summary>
/// Represents an audit log entry that captures changes to an entity's properties, 
/// including what changed, who changed it, and when the change occurred.
/// </summary>
public class AuditHistory
{
    /// <summary>
    /// Gets or sets the unique identifier for the audit history record.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the name of the entity (e.g., Offer, OfferItem) that was changed.
    /// </summary>
    public string EntityName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the primary key property of the changed entity.
    /// </summary>
    public string PrimaryKeyName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the value of the primary key for the changed entity.
    /// </summary>
    public string PrimaryKeyValue { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the property that was changed.
    /// </summary>
    public string ChangedField { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the previous value of the changed property.
    /// </summary>
    public string? OldValue { get; set; }

    /// <summary>
    /// Gets or sets the new value of the changed property.
    /// </summary>
    public string? NewValue { get; set; }

    /// <summary>
    /// Gets or sets the user or process that performed the change.
    /// </summary>
    public string ChangedBy { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date and time when the change occurred (in UTC).
    /// </summary>
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
}
