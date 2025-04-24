using System.Net;

namespace E7.EasyResult.Errors;

internal class FailToCreateObjectError() : AppError("Fail to object Create", ErrorType.FailToCreateObject,
    nameof(FailToCreateObjectError))
{
    public override HttpStatusCode GetHttpStatusCode() => HttpStatusCode.InternalServerError;
    public override List<string?> GetErrorsMessage() => [Message];
}