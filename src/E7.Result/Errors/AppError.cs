using System.Net;

namespace E7.Result.Errors;

/// <summary>
/// Represents an abstract base class for application errors.
/// </summary>
/// <param name="detail">The detail message of the error.</param>
/// <param name="errorType">The type of the error.</param>
/// <param name="errorCodeName">The name of the error code.</param>
public abstract class AppError(string detail, ErrorType errorType, string errorCodeName)
{
    /// <summary>
    /// Gets the detail message of the error.
    /// </summary>
    protected string Detail { get; } = detail;

    /// <summary>
    /// Gets the type of the error.
    /// </summary>
    public ErrorType ErrorType { get; } = errorType;

    /// <summary>
    /// Gets the name of the error code.
    /// </summary>
    public string ErrorCodeName { get; } = errorCodeName;

    /// <summary>
    /// Gets the list of error messages.
    /// </summary>
    /// <returns>A list of error messages.</returns>
    public abstract List<string?> GetErrorsMessage();

    /// <summary>
    /// Gets the HTTP status code associated with the error.
    /// </summary>
    /// <returns>The HTTP status code <see cref="HttpStatusCode"/>.</returns>
    public abstract HttpStatusCode GetHttpStatusCode();
}