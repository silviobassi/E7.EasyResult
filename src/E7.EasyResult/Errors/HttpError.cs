using System.Net;

namespace E7.EasyResult.Errors;

/// <summary>
/// Represents an application error that is specifically intended to be mapped to an HTTP response.
/// This class extends <see cref="AppError"/> to provide semantic integration with HTTP-based systems,
/// enabling consistent translation of application-level errors into HTTP status codes.
/// </summary>
/// <remarks>
/// Use this class as a base for errors that originate in the domain or application layer
/// but need to be surfaced to HTTP clients with an appropriate <see cref="HttpStatusCode"/>.
/// It enforces a contract for HTTP-aware error handling, separating it from domain-only errors
/// that have no concern with HTTP transport semantics.
/// </remarks>
/// <param name="message">A human-readable message describing the error in detail.</param>
/// <param name="appErrorType">The classification of the error using a strongly-typed enumeration.</param>
/// <param name="errorCodeName">A symbolic code name used to uniquely identify the specific error case.</param>
/// <author>Silvio Luiz Bassi</author>
/// <company>Enfatiza7 Consultoria em Tecnologia LTDA</company>
public abstract class HttpError(string message, Enum appErrorType, string errorCodeName)
    : AppError(message, appErrorType, errorCodeName)
{
    /// <summary>
    /// Gets the HTTP status code that corresponds to this error type.
    /// This allows mapping structured application errors directly to HTTP response codes
    /// in APIs and web services.
    /// </summary>
    /// <returns>
    /// An <see cref="HttpStatusCode"/> value that best represents the semantics of the error.
    /// For example, <see cref="HttpStatusCode.NotFound"/> for missing resources,
    /// or <see cref="HttpStatusCode.BadRequest"/> for validation errors.
    /// </returns>
    public abstract HttpStatusCode GetHttpStatusCode();
}