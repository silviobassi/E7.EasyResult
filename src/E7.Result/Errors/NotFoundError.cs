using System.Net;

namespace E7.Result.Errors;

public sealed class NotFoundError()
    : AppError("Object not found.", ErrorType.NotFoundRule, nameof(NotFoundError))
{
    public override List<string?> GetErrorsMessage() => [Detail];

    public override HttpStatusCode GetHttpStatusCode() => HttpStatusCode.NotFound;
}