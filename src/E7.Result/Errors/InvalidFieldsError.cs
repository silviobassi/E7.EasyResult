using System.Net;

namespace E7.Result.Errors;

public sealed class InvalidFieldsError(List<string?> errorsMessages)
    : AppError("Invalid Fields.", ErrorType.ValidationRule, nameof(InvalidFieldsError))
{
    public override List<string?> GetErrorsMessage() => errorsMessages;

    public override HttpStatusCode GetHttpStatusCode() => HttpStatusCode.BadRequest;
}