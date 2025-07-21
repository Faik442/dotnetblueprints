using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBlueprints.SharedKernel.Exceptions;

/// <summary>
/// Exception thrown when a requested entity is not found in the system.
/// </summary>
public class NotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundException"/> class.
    /// </summary>
    public NotFoundException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundException"/> class with a custom message.
    /// </summary>
    /// <param name="message">The error message.</param>
    public NotFoundException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundException"/> class
    /// using the name of the entity and its key.
    /// </summary>
    /// <param name="name">The name of the entity.</param>
    /// <param name="key">The key that was not found.</param>
    public NotFoundException(string name, object key)
        : base(string.Format(ExceptionMessages.EntityNotFound, name, key))
    {
    }
}
