namespace E7.EasyResult.Errors;

/// <summary>
/// Provides an abstract base for all structured application errors,
/// ensuring a consistent model for categorizing, identifying, and communicating issues
/// across various application layers (e.g., domain, application, infrastructure).
/// </summary>
/// <remarks>
/// This record encapsulates essential error metadata: its type, a unique identifier,
/// and a descriptive message. It is designed for extension by more specific error types
/// (e.g., domain errors, validation errors, infrastructure errors), thereby promoting
/// rich error semantics and facilitating centralized error handling strategies.
///
/// The use of a generic <see cref="Enum"/> for the error type enables the integration
/// of domain- or context-specific enumerations for classification, offering robust
/// extensibility without compromising structural integrity.
/// </remarks>
/// <param name="Message">A developer-oriented message detailing the error context, primarily for diagnostics, logging, or internal feedback.</param>
/// <param name="AppErrorType">An enumeration value that classifies the error, such as <c>Validation</c>, <c>NotFound</c>, or <c>BusinessRuleViolation</c>.</param>
/// <param name="ErrorCodeName">A symbolic and unique identifier for the specific error case, useful for logging, API responses, and system-wide referencing.</param>
/// <author>Silvio Luiz Bassi</author>
/// <company>Enfatiza7 Consultoria em Tecnologia LTDA</company>
public abstract record AppError(string Message, Enum AppErrorType, string ErrorCodeName)
{
    /// <summary>
    /// Retrieves a list of user-facing messages that describe the error in a format suitable for UI display or external API consumers.
    /// Each message aims to provide actionable or descriptive information relevant to the error's context.
    /// </summary>
    /// <returns>
    /// A <see cref="List{T}"/> of strings, each representing a user-friendly message related to this error.
    /// Examples include validation hints, reasons for access denial, or instructions for resolution.
    /// </returns>
    public abstract List<string> GetErrorsMessage();
}