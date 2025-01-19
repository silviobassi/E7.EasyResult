using System.Net;
using E7.Result.Errors;

namespace E7.Result.Tests;

internal sealed class TestAppError(string detail, ErrorType errorType, string errorCodeName)
    : AppError(detail, errorType, errorCodeName)
{
    public override List<string?> GetErrorsMessage() => [Detail];

    public override HttpStatusCode GetHttpStatusCode() => HttpStatusCode.BadRequest;
}