using System.Net;

namespace E7.Result.Errors;

/// <summary>
/// Represents an error that occurs when an object is not found.
/// </summary>
public sealed class NotFoundError()
    : AppError("Object not found.", ErrorType.NotFoundRule, nameof(NotFoundError))
{
    /// <summary>
    /// Gets the list of error messages.
    /// </summary>
    /// <returns>A list containing the detail message of the error.</returns>
    public override List<string?> GetErrorsMessage() => [Detail];

    /// <summary>
    /// Gets the HTTP status code associated with the error.
    /// </summary>
    /// <returns>The HTTP status code for not found (404) <see cref="ResultSS{T}"/>.</returns>
    public override HttpStatusCode GetHttpStatusCode() => HttpStatusCode.NotFound;
}