namespace E7.EasyResult.Errors;

/// <summary>
/// Serves as the abstract base class for all structured application errors,
/// offering a consistent model for categorizing, identifying, and communicating issues
/// across different layers of an application (e.g., domain, application, infrastructure).
/// </summary>
/// <remarks>
/// This class encapsulates the core metadata for an error, such as its type,
/// identifier, and descriptive message. It is designed to be extended by more specific
/// error types (e.g., domain errors, validation errors, infrastructure errors),
/// promoting rich error semantics and centralized handling strategies.
///
/// The use of a generic <see cref="Enum"/> for the error type allows domain- or context-specific
/// enumerations to be used for classification, enabling extensibility without sacrificing structure.
/// </remarks>
/// <param name="message">A descriptive, developer-oriented message explaining the error context.</param>
/// <param name="appErrorType">An enumeration value representing the error's classification.</param>
/// <param name="errorCodeName">A symbolic, unique identifier for the error case, used for logging or API responses.</param>
/// <author>Silvio Luiz Bassi</author>
/// <company>Enfatiza7 Consultoria em Tecnologia LTDA</company>
public abstract class AppError(string message, Enum appErrorType, string errorCodeName)
{
    /// <summary>
    /// Gets a descriptive message that explains the error in technical terms.
    /// Typically used for diagnostics, logs, or internal developer feedback.
    /// </summary>
    protected string Message { get; } = message;

    /// <summary>
    /// Gets the classification of the error.
    /// This is typically an enumeration value that categorizes the error type
    /// (e.g., Validation, NotFound, BusinessRuleViolation).
    /// </summary>
    public Enum AppErrorType { get; } = appErrorType;

    /// <summary>
    /// Gets the symbolic code name of the error, which can be used for identifying
    /// and referencing specific error cases across systems and clients.
    /// </summary>
    public string ErrorCodeName { get; } = errorCodeName;

    /// <summary>
    /// Provides a list of user-facing messages that describe the error in a way suitable for UI display or API consumers.
    /// Each message should convey actionable or descriptive information relevant to the context of the error.
    /// </summary>
    /// <returns>
    /// A list of strings representing user-friendly messages related to this error.
    /// These may include validation hints, access denial reasons, or instructions for resolution.
    /// </returns>
    public abstract List<string> GetErrorsMessage();
}