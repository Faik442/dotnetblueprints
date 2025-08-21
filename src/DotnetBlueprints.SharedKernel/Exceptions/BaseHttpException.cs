using System.Net;

namespace DotnetBlueprints.SharedKernel.Exceptions;

/// <summary>
/// The abstract base class for all custom HTTP exceptions in the application.
/// Encapsulates standard HTTP error details such as status code, error message,
/// source system identifier, and correlation ID.
/// Inherit from this class for all API-related exception types to ensure
/// consistent error handling, logging, and response formatting across the application.
/// </summary>
/// <remarks>
/// <para>
/// <b>Usage:</b>
/// <list type="bullet">
///   <item>Derive all your HTTP/API-specific exception types from this class.</item>
///   <item>Manage status codes and other error metadata in a single, unified place.</item>
///   <item>Enables your exception middleware to log and return errors consistently.</item>
/// </list>
/// </para>
/// <para>
/// <b>Example:</b>
/// <code>
/// public class NotFoundException : BaseHttpException
/// {
///     public NotFoundException(string message, string? sourceSystem = null)
///         : base(message, HttpStatusCode.NotFound, sourceSystem) { }
/// }
/// </code>
/// </para>
/// </remarks>
public abstract class BaseHttpException : Exception
{
    public HttpStatusCode StatusCode { get; }

    public string ErrorMessage { get; }

    public string? SourceSystem { get; }

    public string CorrelationId { get; }

    protected BaseHttpException(
        string message,
        HttpStatusCode statusCode,
        string? sourceSystem = null,
        string? correlationId = null)
        : base(message)
    {
        StatusCode = statusCode;
        ErrorMessage = message;
        SourceSystem = sourceSystem;
        CorrelationId = correlationId ?? Guid.NewGuid().ToString("N");
    }
}
