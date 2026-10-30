using E7.EasyResult.Errors;
using E7.EasyResult.Tests.Responses;
using System.Net;

namespace E7.EasyResult.Tests.Services;

public static class ResultSimulatorService
{
    public sealed record ProblemDetailsResponse(
        int StatusCode,
        string Detail,
        string? ErrorCode,
        string? ErrorType,
        IReadOnlyList<Notification> Notifications);

    public static Result<ObjectResponse?> GetResult(bool isSuccess)
        => isSuccess
            ? Result<ObjectResponse?>.Success(new ObjectResponse("32kj42t492gfk59df0fd8f", "123456"))
            : Result<ObjectResponse?>.Failure(new ElementNotFoundError());

    public static Result<ObjectResponse?> CheckResult(int value)
    {
        return value == 0
            ? Result<ObjectResponse?>.Failure(new ElementNotFoundError())
            : Result<ObjectResponse?>.Success(new ObjectResponse("32kj42t492gfk59df0fd8f", "123456"));
    }

    public static ProblemDetailsResponse ToProblemDetails(Failure failure)
    {
        var error = failure.Error;
        var statusCode = error is HttpError httpError
            ? (int)httpError.GetHttpStatusCode()
            : (int)HttpStatusCode.BadRequest;
        if (statusCode is < 400 or > 599)
        {
            statusCode = (int)HttpStatusCode.BadRequest;
        }

        return new ProblemDetailsResponse(
            statusCode,
            error?.Message ?? "Validation failed.",
            error?.ErrorCodeName,
            error?.AppErrorType.ToString(),
            failure.Notifications);
    }
}