using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBlueprints.SharedKernel.Exceptions;

/// <summary>
/// Exception thrown when a requested entity is not found in the system.
/// </summary>
public class NotFoundException : BaseHttpException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundException"/> class
    /// using the name of the entity and its key.
    /// </summary>
    /// <param name="name">The name of the entity.</param>
    /// <param name="key">The key that was not found.</param>
    public NotFoundException(string entity, object value, string? sourceSystem = null, string? correlationId = null)
        : base(string.Format(ExceptionMessages.EntityNotFound(entity, value)), System.Net.HttpStatusCode.NotFound, sourceSystem, correlationId)
    {
    }
}
