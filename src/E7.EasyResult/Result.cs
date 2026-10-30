using E7.EasyResult.Errors;
using SuccessCase = E7.EasyResult.Success;

namespace E7.EasyResult;

/// <summary>Describes a validation or processing notification attached to a failure.</summary>
/// <param name="Code">The code identifying the notification.</param>
/// <param name="Message">The human-readable notification message.</param>
public sealed record Notification(string Code, string Message);

/// <summary>Represents a successful result without a value.</summary>
public sealed record Success;

/// <summary>Represents a successful result carrying a value.</summary>
/// <typeparam name="T">The type of the successful value.</typeparam>
/// <param name="Value">The value produced by the operation.</param>
public sealed record Success<T>(T Value);

/// <summary>Represents a failed result with an optional primary error and notifications.</summary>
/// <remarks>Notifications are copied when the case is constructed so later changes to the source sequence do not alter the failure.</remarks>
public sealed record Failure
{
    /// <summary>Initializes a failure case.</summary>
    /// <param name="error">The optional primary application error.</param>
    /// <param name="notifications">The notifications associated with the failure.</param>
    public Failure(AppError? error, IEnumerable<Notification> notifications)
    {
        Error = error;
        Notifications = [.. notifications];
    }

    /// <summary>Gets the optional primary application error.</summary>
    public AppError? Error { get; }
    /// <summary>Gets the ordered notifications associated with the failure.</summary>
    public IReadOnlyList<Notification> Notifications { get; }
}

/// <summary>Represents a non-generic operation result as either success or failure.</summary>
public union Result(Success, Failure)
{
    /// <summary>Gets whether this result is a success.</summary>
    public bool IsSuccess => this is SuccessCase;
    /// <summary>Gets whether this result is a failure.</summary>
    public bool IsFailure => this is Failure;
    /// <summary>Gets the primary error when this result is a failure; otherwise <see langword="null"/>.</summary>
    public AppError? Error => this switch
    {
        Failure failure => failure.Error,
        SuccessCase => null,
        _ => throw InvalidState()
    };
    /// <summary>Gets the failure notifications, or an empty list for success.</summary>
    public IReadOnlyList<Notification> Notifications => this switch
    {
        Failure failure => failure.Notifications,
        SuccessCase => [],
        _ => throw InvalidState()
    };
    /// <summary>Creates a successful result without a value.</summary>
    public static Result Success() => new Success();
    /// <summary>Creates a failure with an optional primary error.</summary>
    /// <param name="error">The optional primary application error.</param>
    public static Result Failure(AppError? error) => new Failure(error, []);
    /// <summary>Creates a failure containing notifications without a primary error.</summary>
    /// <param name="notifications">The notifications associated with the failure.</param>
    public static Result Failure(IEnumerable<Notification> notifications) =>
        new Failure(null, [.. notifications]);
    /// <summary>Creates a failure containing an optional error and notifications.</summary>
    /// <param name="error">The optional primary application error.</param>
    /// <param name="notifications">The notifications associated with the failure.</param>
    public static Result Failure(AppError? error, IEnumerable<Notification> notifications) =>
        new Failure(error, [.. notifications]);
    /// <summary>Creates a result from an asynchronous value, failing when the value is <see langword="null"/>.</summary>
    /// <typeparam name="T">The type of the asynchronous value.</typeparam>
    /// <param name="value">The task producing the value.</param>
    /// <returns>A successful result with the value or a creation failure.</returns>
    public static async Task<Result<T>> Create<T>(Task<T> value)
    {
        var result = await value.ConfigureAwait(false);
        return result is null
            ? Result<T>.Failure(new FailToCreateObjectError())
            : Result<T>.Success(result);
    }
    /// <summary>Checks whether the primary error has the specified classification.</summary>
    /// <param name="appErrorType">The classification to compare.</param>
    /// <returns><see langword="true"/> when the primary error matches; otherwise <see langword="false"/>.</returns>
    public bool IsErrorType(Enum appErrorType) =>
        Error?.AppErrorType.Equals(appErrorType) ?? false;
    /// <summary>Returns a human-readable representation of this result.</summary>
    public override string ToString() => this switch
    {
        SuccessCase => "Success",
        Failure failure => FormatFailure(failure),
        _ => throw InvalidState()
    };
    private static string FormatFailure(Failure failure) =>
        failure.Notifications.Count == 0
            ? $"Failure: {failure.Error}"
            : $"Failure: {failure.Notifications.Count} notifications. Primary Error: {failure.Error?.ToString() ?? "None"}";
    internal static InvalidOperationException InvalidState() =>
        new("The result is not initialized with a valid case.");
}

/// <summary>Represents a value-bearing operation result as either success or failure.</summary>
/// <typeparam name="T">The type of the successful value.</typeparam>
public union Result<T>(Success<T>, Failure)
{
    /// <summary>Gets whether this result is a success.</summary>
    public bool IsSuccess => this is Success<T>;
    /// <summary>Gets whether this result is a failure.</summary>
    public bool IsFailure => this is Failure;
    /// <summary>Gets the primary error when this result is a failure; otherwise <see langword="null"/>.</summary>
    public AppError? Error => this switch
    {
        Failure failure => failure.Error,
        Success<T> => null,
        _ => throw Result.InvalidState()
    };
    /// <summary>Gets the failure notifications, or an empty list for success.</summary>
    public IReadOnlyList<Notification> Notifications => this switch
    {
        Failure failure => failure.Notifications,
        Success<T> => [],
        _ => throw Result.InvalidState()
    };
    /// <summary>Creates a successful result containing the specified value.</summary>
    /// <param name="value">The value produced by the operation.</param>
    public static Result<T> Success(T value) => new Success<T>(value);
    /// <summary>Creates a failure with an optional primary error.</summary>
    /// <param name="error">The optional primary application error.</param>
    public static Result<T> Failure(AppError? error) => new Failure(error, []);
    /// <summary>Creates a failure containing notifications without a primary error.</summary>
    /// <param name="notifications">The notifications associated with the failure.</param>
    public static Result<T> Failure(IEnumerable<Notification> notifications) =>
        new Failure(null, [.. notifications]);
    /// <summary>Creates a failure containing an optional error and notifications.</summary>
    /// <param name="error">The optional primary application error.</param>
    /// <param name="notifications">The notifications associated with the failure.</param>
    public static Result<T> Failure(AppError? error, IEnumerable<Notification> notifications) =>
        new Failure(error, [.. notifications]);
    /// <summary>Creates a result from a value, failing when the value is <see langword="null"/>.</summary>
    /// <param name="value">The value to wrap.</param>
    /// <returns>A successful result with the value or a creation failure.</returns>
    public static Result<T> Create(T value) =>
        value is null ? Failure(new FailToCreateObjectError()) : Success(value);
    /// <summary>Returns a human-readable representation of this result.</summary>
    public override string ToString() => this switch
    {
        Success<T> success => $"Success: {success.Value}",
        Failure failure => failure.Notifications.Count == 0
            ? $"Failure: {failure.Error}"
            : $"Failure: {failure.Notifications.Count} notifications. Primary Error: {failure.Error?.ToString() ?? "None"}",
        _ => throw Result.InvalidState()
    };
}
