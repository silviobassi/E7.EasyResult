namespace E7.EasyResult.Errors;

/// <summary>
/// Enumerates the standard categories of application errors used in conjunction with the result pattern.
/// These categories help classify failures for clearer communication, diagnostics, and HTTP response mapping.
/// </summary>
/// <remarks>
/// Each error type is assigned a specific code for traceability and consistency across the system,
/// particularly useful in result-based flows and domain-to-HTTP translation layers.
/// </remarks>
/// <author>Silvio Luiz Bassi</author>
/// <company>Enfatiza7 Consultoria em Tecnologia LTDA</company>
public enum HttpErrorType
{
    /// <summary>
    /// Represents an error caused by an invalid or malformed request.
    /// Typically corresponds to HTTP 400 (Bad Request).
    /// </summary>
    BadRequestRule = 100,

    /// <summary>
    /// Represents a violation of business rules or domain logic.
    /// Used to indicate that an operation, although syntactically correct, is not allowed.
    /// </summary>
    BusinessRule = 200,

    /// <summary>
    /// Represents a conflict in the application's state, usually due to concurrency or data integrity violations.
    /// Typically maps to HTTP 409 (Conflict).
    /// </summary>
    ConflictRule = 300,

    /// <summary>
    /// Represents an error where the user is authenticated but does not have the necessary permissions to perform the action.
    /// Corresponds to HTTP 403 (Forbidden).
    /// </summary>
    ForbiddenAccessRule = 400,

    /// <summary>
    /// Represents an error where a requested resource could not be found.
    /// Corresponds to HTTP 404 (Not Found).
    /// </summary>
    NotFoundRule = 500,

    /// <summary>
    /// Represents a data validation failure, such as format errors or missing required values.
    /// Typically used for client-side input validation.
    /// </summary>
    ValidationRule = 600,

    /// <summary>
    /// Represents an error where the user is not authenticated.
    /// Corresponds to HTTP 401 (Unauthorized).
    /// </summary>
    UnauthorizedAccess = 700,

    /// <summary>
    /// Represents a failure during the creation of a new object or resource.
    /// Useful for capturing construction or instantiation issues at runtime.
    /// </summary>
    FailToCreateObject = 800,
}