using DotnetBlueprints.SharedKernel.Abstractions;
using DotnetBlueprints.SharedKernel.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DotnetBlueprints.SharedKernel.Infrastructure;

/// <summary>
/// EF Core interceptor that automatically populates audit fields
/// (CreatedBy/CreatedDate/UpdatedBy/UpdatedDate/DeletedBy/DeletedDate) and
/// converts hard deletes into soft deletes.
/// </summary>
public sealed class AuditInterceptor : SaveChangesInterceptor
{
    private readonly IUserContext _user;

    /// <summary>Initializes a new instance of the <see cref="AuditInterceptor"/> class.</summary>
    public AuditInterceptor(IUserContext user) => _user = user;

    /// <inheritdoc />
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        ApplyAudit(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    /// <inheritdoc />
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ApplyAudit(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <summary>
    /// Applies audit and soft-delete rules to tracked entities inheriting from <see cref="BaseEntity"/>.
    /// </summary>
    private void ApplyAudit(DbContext? ctx)
    {
        if (ctx is null) return;

        var now = DateTime.UtcNow;
        var by = _user.UserId ?? _user.UserName ?? "system";

        foreach (var e in ctx.ChangeTracker.Entries<BaseEntity>())
        {
            switch (e.State)
            {
                case EntityState.Added:
                    e.Entity.CreatedDate = now;
                    e.Entity.CreatedBy = by;
                    break;

                case EntityState.Modified:
                    e.Entity.UpdatedDate = now;
                    e.Entity.UpdatedBy = by;
                    break;

                case EntityState.Deleted:
                    e.State = EntityState.Modified;
                    e.Entity.IsDeleted = true;
                    e.Entity.DeletedDate = now;
                    e.Entity.DeletedBy = by;
                    break;
            }
        }
    }
}
