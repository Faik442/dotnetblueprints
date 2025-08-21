using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBlueprints.SharedKernel.Exceptions;

/// <summary>
/// Exception thrown when attempting to set an entity's status to its current value.
/// </summary>
public class ValueAlreadySetException : BaseHttpException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValueAlreadySetException"/> class.
    /// </summary>
    /// <param name="entityName">The name of the entity.</param>
    /// <param name="value">The current status of value.</param>
    public ValueAlreadySetException(string entityName, object value, string? sourceSystem = null, string? correlationId = null)
        : base(string.Format(ExceptionMessages.ValueAlreadySet(entityName, value)), System.Net.HttpStatusCode.Conflict, sourceSystem, correlationId)
    {
    }
}
