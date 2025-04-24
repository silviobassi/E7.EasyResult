using System.Net;

namespace E7.EasyResult.Errors;

/// <summary>
/// Represents a standardized base class for application-level errors,
/// providing a consistent structure for error categorization, identification, and messaging.
/// </summary>
/// <param name="message">A human-readable message describing the error.</param>
/// <param name="errorType">The high-level classification of the error (e.g., Validation, NotFound, Conflict...).</param>
/// <param name="errorCodeName">A symbolic code name used to identify the specific error.</param>
/// <remarks>
/// This class is intended to be inherited by domain-specific error types, enabling rich,
/// structured error handling across application boundaries (e.g., domain, application, API).
/// </remarks>
/// <author>Silvio Luiz Bassi</author>
/// <company>Enfatiza7 Consultoria em Tecnologia LTDA</company>
public abstract class AppError(string? message, ErrorType errorType, string errorCodeName)
{
    /// <summary>
    /// Gets a human-readable message that describes the error in detail.
    /// This message is intended for developers and internal diagnostics.
    /// </summary>
    protected string? Message { get; } = message;

    /// <summary>
    /// Gets the classification type of the error, typically used to drive decision-making
    /// logic or determine the appropriate HTTP status code or UI treatment.
    /// </summary>
    public ErrorType ErrorType { get; } = errorType;

    /// <summary>
    /// Gets the symbolic name of the error code.
    /// Useful for identifying specific errors in logs, telemetry, or external clients.
    /// </summary>
    public string ErrorCodeName { get; } = errorCodeName;

    /// <summary>
    /// Returns the corresponding HTTP status code for the error.
    /// This allows mapping of domain/application errors directly to HTTP response codes.
    /// </summary>
    /// <returns>
    /// An <see cref="HttpStatusCode"/> that semantically represents the error.
    /// </returns>
    public abstract HttpStatusCode GetHttpStatusCode();

    /// <summary>
    /// Returns a list of user-facing error messages related to this error.
    /// These messages may be presented to end users or consumed by client applications.
    /// </summary>
    /// <returns>
    /// A collection of strings that describe the error in a user-friendly manner.
    /// </returns>
    public abstract List<string?> GetErrorsMessage();
}