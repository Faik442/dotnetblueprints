using System.Reflection;

namespace DotnetBlueprints.SharedKernel.Audit;

/// <summary>
/// Provides extension methods for auditing entity changes.
/// </summary>
public static class AuditExtensions
{
    /// <summary>
    /// Compares two entities and returns a list of detected changes for audit logging.
    /// Only properties decorated with [Auditable] are checked.
    /// </summary>
    public static List<AuditHistory> GetAuditChanges<T>(
        this T oldEntity,
        T newEntity,
        string changedBy)
        where T : class
    {
        var changes = new List<AuditHistory>();
        var type = typeof(T);
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in properties)
        {
            if (prop.GetCustomAttribute<AuditableAttribute>() == null)
                continue;

            var oldValue = prop.GetValue(oldEntity)?.ToString();
            var newValue = prop.GetValue(newEntity)?.ToString();

            if (oldValue != newValue)
            {
                changes.Add(new AuditHistory
                {
                    EntityName = type.Name,
                    PrimaryKeyName = "Id",
                    PrimaryKeyValue = type.GetProperty("Id")?.GetValue(newEntity)?.ToString() ?? "",
                    ChangedField = prop.Name,
                    OldValue = oldValue,
                    NewValue = newValue,
                    ChangedBy = changedBy,
                    ChangedAt = DateTime.UtcNow
                });
            }
        }
        return changes;
    }
}

