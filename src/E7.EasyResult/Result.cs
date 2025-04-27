using E7.EasyResult.Errors;

namespace E7.EasyResult;

/// <summary>
/// Represents the outcome of an operation that can either succeed or fail,
/// without returning a value on success.
/// Commonly used in scenarios where only the success/failure state and potential error matter.
/// </summary>
public class Result
{
    /// <summary>
    /// Indicates whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Indicates whether the operation failed.
    /// This is the inverse of <see cref="IsSuccess"/>.
    /// </summary>
    public bool IsFailure { get; }

    /// <summary>
    /// Gets the error associated with the result, if the operation failed.
    /// </summary>
    public AppError? Error { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// </summary>
    /// <param name="isSuccess">Specifies whether the operation succeeded.</param>
    /// <param name="error">The error associated with the failure, or null if successful.</param>
    protected Result(bool isSuccess, AppError? error)
    {
        IsSuccess = isSuccess;
        IsFailure = !isSuccess;
        if (error != null) Error = error;
    }

    /// <summary>
    /// Creates a <see cref="Result{TValue}"/> from a given value.
    /// If the value is null, returns a failure with a <see cref="FailToCreateObjectError"/>.
    /// </summary>
    public static Result<TValue> Create<TValue>(TValue value)
        => value is not null ? Result<TValue>.Success(value) : value;

    /// <summary>
    /// Asynchronously creates a <see cref="Result{T}"/> from a task result.
    /// If the resolved value is null, returns a failure with a <see cref="FailToCreateObjectError"/>.
    /// </summary>
    public static async Task<Result<T>> Create<T>(Task<T> value)
    {
        var result = await value;
        return result is not null ? Result<T>.Success(result) : Result<T>.Failure(new FailToCreateObjectError());
    }

    /// <summary>
    /// Creates a successful result without any associated value.
    /// </summary>
    public static Result Success() => new(true, null);

    /// <summary>
    /// Creates a failed result with the specified error.
    /// </summary>
    public static Result Failure(AppError? error) => new(false, error);

    /// <summary>
    /// Implicitly converts an <see cref="AppError"/> into a failed result.
    /// </summary>
    public static implicit operator Result(AppError? error) => Failure(error);

    /// <summary>
    /// Checks whether the result's error matches a given error type.
    /// </summary>
    public bool IsErrorType(Enum appErrorType) => Error?.AppErrorType.Equals(appErrorType) ?? false;

    /// <summary>
    /// Returns a string describing the result's state.
    /// </summary>
    public override string ToString() => IsSuccess ? "Success" : $"Failure: {Error}";
}

/// <summary>
/// Represents the outcome of an operation that returns a value upon success.
/// Encapsulates both the success/failure state and the associated result data.
/// </summary>
/// <typeparam name="T">The type of the result value returned upon success.</typeparam>
public class Result<T> : Result
{
    /// <summary>
    /// Gets the value returned by the operation.
    /// Only meaningful if <see cref="Result.IsSuccess"/> is <c>true</c>.
    /// </summary>
    public T Value { get; }

    private Result(T value, bool isSuccess, AppError? error) : base(isSuccess, error)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a successful result with the specified value.
    /// </summary>
    public static Result<T> Success(T value) => new(value, true, null);

    /// <summary>
    /// Creates a failed result with the specified error.
    /// </summary>
    public new static Result<T> Failure(AppError error) => new(default!, false, error);

    /// <summary>
    /// Implicitly converts a value to a successful result.
    /// </summary>
    public static implicit operator Result<T>(T value) => Success(value);

    /// <summary>
    /// Implicitly converts an error to a failed result.
    /// </summary>
    public static implicit operator Result<T>(AppError error) => Failure(error);

    /// <summary>
    /// Returns a string describing the result and its value or error.
    /// </summary>
    public override string ToString() => IsSuccess ? $"Success: {Value}" : $"Failure: {Error}";
}