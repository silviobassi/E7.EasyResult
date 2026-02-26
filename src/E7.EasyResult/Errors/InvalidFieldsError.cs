using System.Net;

namespace E7.EasyResult.Errors;

/// <summary>
/// Represents an application error that occurs when one or more input fields fail validation.
/// Typically used for client-side input validation scenarios.
/// </summary>
/// <param name="ErrorsMessages">
/// A list of messages describing each validation failure encountered. These messages are typically shown ao usuário final ou retornadas em uma resposta de API.
/// </param>
/// <remarks>
/// This error is commonly mapped to HTTP status code 400 (Bad Request) to indicate client-side input issues.
/// </remarks>
/// <author>Silvio Luiz Bassi</author>
/// <company>Enfatiza7 Consultoria em Tecnologia LTDA</company>
public sealed record InvalidFieldsError(List<string> ErrorsMessages)
    : HttpError("Invalid fields.", HttpErrorType.ValidationRule, nameof(InvalidFieldsError))
{
    /// <summary>
    /// Returns the collection of specific validation error messages associated with this error.
    /// </summary>
    /// <returns>
    /// A <see cref="List{T}"/> of strings containing the details of each validation failure.
    /// </returns>
    public override List<string> GetErrorsMessage() => ErrorsMessages;

    /// <summary>
    /// Gets the HTTP status code that represents this validation error.
    /// </summary>
    /// <returns>
    /// The <see cref="HttpStatusCode.BadRequest"/> status code (400), indicating that the client submitted invalid input.
    /// </returns>
    public override HttpStatusCode GetHttpStatusCode() => HttpStatusCode.BadRequest;
}