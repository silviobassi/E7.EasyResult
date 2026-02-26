using E7.EasyResult.Errors;

namespace E7.EasyResult;

/// <summary>
/// Represents a domain or application validation notification.
/// Typically used in the Notification Pattern to accumulate multiple non-fatal business rule violations.
/// </summary>
/// <param name="Code">A unique code identifying the specific validation rule or error.</param>
/// <param name="Message">A descriptive message explaining the validation failure.</param>
/// <author>Silvio Luiz Bassi</author>
/// <company>Enfatiza7 Consultoria em Tecnologia LTDA</company>
public sealed record Notification(string Code, string Message);

/// <summary>
/// Represents the outcome of an operation that can either succeed or fail,
/// without returning a value on success. Encapsulates the success state, 
/// a primary application error, and an optional collection of validation notifications.
/// </summary>
public class Result
{
    /// <summary>
    /// Indicates whether the operation completed successfully.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Indicates whether the operation resulted in a failure.
    /// </summary>
    public bool IsFailure { get; }

    /// <summary>
    /// Gets the primary error associated with the failure, if any.
    /// </summary>
    public AppError? Error { get; }

    /// <summary>
    /// Gets a read-only collection of validation notifications accumulated during the operation.
    /// </summary>
    public IReadOnlyCollection<Notification> Notifications { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// </summary>
    /// <param name="isSuccess">Indicates whether the operation was successful.</param>
    /// <param name="error">The primary error associated with the failure, or null if successful.</param>
    /// <param name="notifications">An optional list of validation notifications.</param>
    protected Result(bool isSuccess, AppError? error, List<Notification>? notifications = null)
    {
        IsSuccess = isSuccess;
        IsFailure = !isSuccess;
        Error = error;
        Notifications = notifications?.AsReadOnly() ?? new List<Notification>().AsReadOnly();
    }

    /// <summary>
    /// Creates a successful <see cref="Result{TValue}"/> if the provided value is not null;
    /// otherwise, implicitly converts to a failure.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value to wrap in a result.</param>
    /// <returns>A successful result containing the value, or a failure if the value is null.</returns>
    public static Result<TValue> Create<TValue>(TValue value)
        => value is not null ? Result<TValue>.Success(value) : value;

    /// <summary>
    /// Asynchronously creates a <see cref="Result{T}"/> from a task. 
    /// If the resolved value is null, returns a failure indicating the object creation failed.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The asynchronous task returning the value.</param>
    /// <returns>A task resolving to a successful result or a failure if the value is null.</returns>
    public static async Task<Result<T>> Create<T>(Task<T> value)
    {
        var result = await value;
        return result is not null ? Result<T>.Success(result) : Result<T>.Failure(new FailToCreateObjectError());
    }

    /// <summary>
    /// Creates a successful result without a return value.
    /// </summary>
    /// <returns>A successful <see cref="Result"/>.</returns>
    public static Result Success() => new(true, null);

    /// <summary>
    /// Creates a failed result with a primary application error.
    /// </summary>
    /// <param name="error">The error detailing the failure.</param>
    /// <returns>A failed <see cref="Result"/>.</returns>
    private static Result Failure(AppError? error) => new(false, error);

    /// <summary>
    /// Creates a failed result populated entirely by a list of validation notifications.
    /// </summary>
    /// <param name="notifications">The list of validation notifications.</param>
    /// <returns>A failed <see cref="Result"/>.</returns>
    private static Result Failure(List<Notification> notifications) => new(false, null, notifications);

    /// <summary>
    /// Creates a failed result containing both a primary application error and a list of detailed notifications.
    /// </summary>
    /// <param name="error">The primary error.</param>
    /// <param name="notifications">The detailed validation notifications.</param>
    /// <returns>A failed <see cref="Result"/>.</returns>
    public static Result Failure(AppError? error, List<Notification> notifications) => new(false, error, notifications);

    /// <summary>
    /// Implicitly converts an <see cref="AppError"/> into a failed result.
    /// </summary>
    public static implicit operator Result(AppError? error) => Failure(error);

    /// <summary>
    /// Implicitly converts a list of <see cref="Notification"/> into a failed result.
    /// </summary>
    public static implicit operator Result(List<Notification> notifications) => Failure(notifications);

    /// <summary>
    /// Checks if the primary error matches a specific <see cref="Enum"/> error type.
    /// </summary>
    /// <param name="appErrorType">The error type to compare against.</param>
    /// <returns>True if the error type matches; otherwise, false.</returns>
    public bool IsErrorType(Enum appErrorType) => Error?.AppErrorType.Equals(appErrorType) ?? false;

    /// <summary>
    /// Returns a string representation of the result state, including error and notification details if it is a failure.
    /// </summary>
    public override string ToString()
    {
        if (IsSuccess) return "Success";
        return Notifications.Count switch
        {
            0 => $"Failure: {Error}",
            _ => $"Failure: {Notifications.Count} notifications. Primary Error: {Error?.ToString() ?? "None"}"
        };
    }
}

/// <summary>
/// Represents the outcome of an operation that returns a value upon success.
/// Encapsulates the value alongside the success state, primary error, and any notifications.
/// </summary>
/// <typeparam name="T">The type of the underlying value.</typeparam>
public class Result<T> : Result
{
    /// <summary>
    /// Gets the value returned by the operation.
    /// Only meaningful if <see cref="Result.IsSuccess"/> is true.
    /// </summary>
    public T Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{T}"/> class.
    /// </summary>
    private Result(T value, bool isSuccess, AppError? error, List<Notification>? notifications = null)
        : base(isSuccess, error, notifications)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a successful result containing the provided value.
    /// </summary>
    /// <param name="value">The value of the successful operation.</param>
    /// <returns>A successful <see cref="Result{T}"/>.</returns>
    public static Result<T> Success(T value) => new(value, true, null);

    /// <summary>
    /// Creates a failed result with a primary application error.
    /// </summary>
    /// <param name="error">The error detailing the failure.</param>
    /// <returns>A failed <see cref="Result{T}"/>.</returns>
    public static Result<T> Failure(AppError error) => new(default!, false, error);

    /// <summary>
    /// Creates a failed result populated entirely by a list of validation notifications.
    /// </summary>
    /// <param name="notifications">The list of validation notifications.</param>
    /// <returns>A failed <see cref="Result{T}"/>.</returns>
    public static Result<T> Failure(List<Notification> notifications) => new(default!, false, null, notifications);

    /// <summary>
    /// Creates a failed result containing both a primary application error and a list of detailed notifications.
    /// </summary>
    /// <param name="error">The primary error.</param>
    /// <param name="notifications">The detailed validation notifications.</param>
    /// <returns>A failed <see cref="Result{T}"/>.</returns>
    public new static Result<T> Failure(AppError? error, List<Notification> notifications) =>
        new(default!, false, error, notifications);

    /// <summary>
    /// Implicitly converts a value into a successful result.
    /// </summary>
    public static implicit operator Result<T>(T value) => Success(value);

    /// <summary>
    /// Implicitly converts an <see cref="AppError"/> into a failed result.
    /// </summary>
    public static implicit operator Result<T>(AppError error) => Failure(error);

    /// <summary>
    /// Implicitly converts a list of <see cref="Notification"/> into a failed result.
    /// </summary>
    public static implicit operator Result<T>(List<Notification> notifications) => Failure(notifications);

    /// <summary>
    /// Returns a string representation of the result state, including the value on success, 
    /// or error and notification details on failure.
    /// </summary>
    public override string ToString()
    {
        if (IsSuccess) return $"Success: {Value}";
        return Notifications.Count switch
        {
            0 => $"Failure: {Error}",
            _ => $"Failure: {Notifications.Count} notifications. Primary Error: {Error?.ToString() ?? "None"}"
        };
    }
}