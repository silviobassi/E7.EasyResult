using System.Net;
using E7.EasyResult;
using E7.EasyResult.Errors;

var notFound = EndpointWithHttpError();
Assert(notFound.StatusCode == 404, "HttpError status should be preserved.");
Assert(notFound.Detail == "Order was not found.", "HttpError message should be preserved.");

var validation = EndpointWithValidationFailure();
Assert(validation.StatusCode == 400, "Application errors should use BadRequest.");
Assert(validation.Notifications.Count == 1, "Notifications should be preserved.");

var commonError = EndpointWithCommonError();
Assert(commonError.StatusCode == 400, "Application errors should use BadRequest.");
Assert(commonError.Detail == "Order is invalid.", "Application error message should be preserved.");
Assert(commonError.ErrorCode == nameof(OrderInvalidError), "Application error code should be preserved.");
Assert(commonError.ErrorType == HttpErrorType.BadRequest.ToString(), "Application error type should be preserved.");

var equivalent = EndpointWithSameFailure();
Assert(equivalent.StatusCode == notFound.StatusCode, "Equivalent endpoints should use the same status.");
Assert(equivalent.Detail == notFound.Detail, "Equivalent endpoints should use the same detail.");
Assert(equivalent.Notifications.SequenceEqual(notFound.Notifications), "Equivalent endpoints should preserve notifications.");

var addedError = EndpointWithAddedHttpError();
Assert(addedError.StatusCode == 409, "New HttpError types should use the global handler.");

static ProblemDetailsResponse EndpointWithHttpError() =>
    Result<Order>.Failure(new OrderNotFoundError())
        .Match(
            onSuccess: order => new ProblemDetailsResponse(200, $"Order {order.Id}", null, null, []),
            onFailure: (Failure failure) => HandleFailure(failure));

static ProblemDetailsResponse EndpointWithValidationFailure() =>
    Result<Order>.Failure(
            [new Notification("CustomerId", "Customer is required.")])
        .Match(
            onSuccess: order => new ProblemDetailsResponse(200, $"Order {order.Id}", null, null, []),
            onFailure: (Failure failure) => HandleFailure(failure));

static ProblemDetailsResponse EndpointWithCommonError() =>
    Result<Order>.Failure(new OrderInvalidError())
        .Match(
            onSuccess: order => new ProblemDetailsResponse(200, $"Order {order.Id}", null, null, []),
            onFailure: (Failure failure) => HandleFailure(failure));

static ProblemDetailsResponse EndpointWithSameFailure() =>
    Result<Order>.Failure(new OrderNotFoundError())
        .Match(
            onSuccess: order => new ProblemDetailsResponse(200, $"Order {order.Id}", null, null, []),
            onFailure: (Failure failure) => HandleFailure(failure));

static ProblemDetailsResponse EndpointWithAddedHttpError() =>
    Result<Order>.Failure(new OrderConflictError())
        .Match(
            onSuccess: order => new ProblemDetailsResponse(200, $"Order {order.Id}", null, null, []),
            onFailure: (Failure failure) => HandleFailure(failure));

static ProblemDetailsResponse HandleFailure(Failure failure)
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

static void Assert(bool condition, string message)
{
    if (!condition)
    {
        throw new InvalidOperationException(message);
    }
}

internal sealed record ProblemDetailsResponse(
    int StatusCode,
    string Detail,
    string? ErrorCode,
    string? ErrorType,
    IReadOnlyList<Notification> Notifications);

internal sealed record Order(string Id);

internal sealed record OrderNotFoundError()
    : HttpError("Order was not found.", HttpErrorType.NotFound, nameof(OrderNotFoundError))
{
    public override List<string> GetErrorsMessage() => [Message];

    public override HttpStatusCode GetHttpStatusCode() => HttpStatusCode.NotFound;
}

internal sealed record OrderInvalidError()
    : AppError("Order is invalid.", HttpErrorType.BadRequest, nameof(OrderInvalidError))
{
    public override List<string> GetErrorsMessage() => [Message];
}

internal sealed record OrderConflictError()
    : HttpError("Order is already being processed.", HttpErrorType.Conflict, nameof(OrderConflictError))
{
    public override List<string> GetErrorsMessage() => [Message];

    public override HttpStatusCode GetHttpStatusCode() => HttpStatusCode.Conflict;
}
