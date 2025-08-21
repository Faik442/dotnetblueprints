namespace DotnetBlueprints.SharedKernel.Exceptions;

/// <summary>
/// Exception thrown when one or more validation rules are violated for a request or entity.
/// Typically indicates bad input, missing required fields, or other business validation errors.
/// </summary>
public class ValidationException : BaseHttpException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class.
    /// </summary>
    /// <param name="message">A description of the validation error.</param>
    /// <param name="sourceSystem">Optional: The originating system or context.</param>
    /// <param name="correlationId">Optional: A correlation identifier for tracing the error.</param>
    public ValidationException(string message, string? sourceSystem = null, string? correlationId = null)
        : base(message, System.Net.HttpStatusCode.BadRequest, sourceSystem, correlationId)
    {
    }
}

