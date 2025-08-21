using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBlueprints.SharedKernel.Audit;

/// <summary>
/// Defines the contract for persisting audit history records to a data store.
/// </summary>
public interface IAuditHistoryRepository
{
    /// <summary>
    /// Adds a new audit history record to the repository.
    /// </summary>
    /// <param name="history">The audit history record to add.</param>
    void Add(AuditHistory history);

    /// <summary>
    /// Persists all pending audit history changes to the data store.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
