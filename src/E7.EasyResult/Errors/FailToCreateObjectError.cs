using System.Diagnostics;
using System.Net;

namespace E7.EasyResult.Errors;

/// <summary>
/// Represents an application error that occurs when an object or resource fails to be created.
/// Typically used for signaling unexpected failures during instantiation, persistence, or resource allocation.
/// </summary>
/// <remarks>
/// This error corresponds to an internal server issue and is usually mapped to HTTP 500 in API contexts.
/// </remarks>
/// <author>Silvio Luiz Bassi</author>
/// <company>Enfatiza7 Consultoria em Tecnologia LTDA</company>
internal class FailToCreateObjectError()
    : HttpError("Failed to create object.", HttpErrorType.FailToCreateObject, nameof(FailToCreateObjectError))
{
    /// <summary>
    /// Returns the HTTP status code that best represents this type of error.
    /// </summary>
    /// <returns>
    /// The <see cref="HttpStatusCode.InternalServerError"/> status code (500), indicating a server-side failure.
    /// </returns>
    public override HttpStatusCode GetHttpStatusCode() => HttpStatusCode.InternalServerError;

    /// <summary>
    /// Returns a list of descriptive error messages associated with this failure.
    /// </summary>
    /// <returns>
    /// A list containing a single message that describes the error.
    /// </returns>
    public override List<string> GetErrorsMessage() => [Message];
}