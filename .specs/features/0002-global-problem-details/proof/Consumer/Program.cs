using System.Net;
using E7.EasyResult;
using E7.EasyResult.Errors;

var notFound = EndpointWithHttpError();
Assert(notFound.StatusCode == 404, "HttpError status should be preserved.");
Assert(notFound.Detail == "Order was not found.", "HttpError message should be preserved.");

var validation = EndpointWithValidationFailure();
Assert(validation.StatusCode == 400, "Application errors should use BadRequest.");
Assert(validation.Notifications.Count == 1, "Notifications should be preserved.");

var addedError = EndpointWithAddedHttpError();
Assert(addedError.StatusCode == 409, "New HttpError types should use the global handler.");

static ProblemDetailsResponse EndpointWithHttpError() =>
    Result<Order>.Failure(new OrderNotFoundError())
        .Match(
            onSuccess: order => new ProblemDetailsResponse(200, $"Order {order.Id}", []),
            onFailure: (Failure failure) => HandleFailure(failure));

static ProblemDetailsResponse EndpointWithValidationFailure() =>
    Result<Order>.Failure(
            [new Notification("CustomerId", "Customer is required.")])
        .Match(
            onSuccess: order => new ProblemDetailsResponse(200, $"Order {order.Id}", []),
            onFailure: (Failure failure) => HandleFailure(failure));

static ProblemDetailsResponse EndpointWithAddedHttpError() =>
    Result<Order>.Failure(new OrderConflictError())
        .Match(
            onSuccess: order => new ProblemDetailsResponse(200, $"Order {order.Id}", []),
            onFailure: (Failure failure) => HandleFailure(failure));

static ProblemDetailsResponse HandleFailure(Failure failure)
{
    var error = failure.Error;
    var statusCode = error is HttpError httpError
        ? (int)httpError.GetHttpStatusCode()
        : (int)HttpStatusCode.BadRequest;

    return new ProblemDetailsResponse(
        statusCode,
        error?.Message ?? "Validation failed.",
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
    IReadOnlyList<Notification> Notifications);

internal sealed record Order(string Id);

internal sealed record OrderNotFoundError()
    : HttpError("Order was not found.", HttpErrorType.NotFound, nameof(OrderNotFoundError))
{
    public override List<string> GetErrorsMessage() => [Message];

    public override HttpStatusCode GetHttpStatusCode() => HttpStatusCode.NotFound;
}

internal sealed record OrderConflictError()
    : HttpError("Order is already being processed.", HttpErrorType.Conflict, nameof(OrderConflictError))
{
    public override List<string> GetErrorsMessage() => [Message];

    public override HttpStatusCode GetHttpStatusCode() => HttpStatusCode.Conflict;
}
