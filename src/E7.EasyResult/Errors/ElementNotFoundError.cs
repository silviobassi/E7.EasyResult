using System.Net;

namespace E7.EasyResult.Errors;

/// <summary>
/// Represents a domain-specific error indicating that a requested element or resource could not be found.
/// This error typically maps to HTTP 404 and is useful in both domain and API contexts.
/// </summary>
/// <remarks>
/// Use this error when an entity or record is expected but does not exist in the current context or data store.
/// </remarks>
/// <author>Silvio Luiz Bassi</author>
/// <company>Enfatiza7 Consultoria em Tecnologia LTDA</company>
public sealed record ElementNotFoundError()
    : HttpError("Object not found.", HttpErrorType.NotFoundRule, nameof(ElementNotFoundError))
{
    /// <summary>
    /// Returns a collection of user-facing error messages related to this error.
    /// In this case, it contains a single message describing the missing object.
    /// </summary>
    /// <returns>
    /// A list with a single entry: the detail message of the error.
    /// </returns>
    public override List<string> GetErrorsMessage() => [Message];

    /// <summary>
    /// Returns the HTTP status code that corresponds to this error type.
    /// Typically used in API responses to signal that the requested resource was not found.
    /// </summary>
    /// <returns>
    /// The <see cref="HttpStatusCode.NotFound"/> status code (404).
    /// </returns>
    public override HttpStatusCode GetHttpStatusCode() => HttpStatusCode.NotFound;
}