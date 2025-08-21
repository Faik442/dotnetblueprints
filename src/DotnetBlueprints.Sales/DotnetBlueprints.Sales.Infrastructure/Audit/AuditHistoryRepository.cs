using DotnetBlueprints.SharedKernel.Audit;
using System.Threading;
using System.Threading.Tasks;

namespace DotnetBlueprints.Sales.Infrastructure.Audit;

/// <summary>
/// Implements <see cref="IAuditHistoryRepository"/> to persist audit history records
/// using the application's database context.
/// </summary>
public class AuditHistoryRepository : IAuditHistoryRepository
{
    private readonly SalesDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuditHistoryRepository"/> class.
    /// </summary>
    /// <param name="context">The database context used to persist audit history records.</param>
    public AuditHistoryRepository(SalesDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Adds a new audit history record to the underlying data context.
    /// </summary>
    /// <param name="history">The audit history record to add.</param>
    public void Add(AuditHistory history)
    {
        _context.AuditHistories.Add(history);
    }

    /// <summary>
    /// Saves all pending changes, including audit history records, to the database.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous save operation.</returns>
    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
