using E7.EasyResult.Errors;

namespace E7.EasyResult;

/// <summary>
/// Represents the result of an operation, indicating success or failure.
/// </summary>
public class Result
{
    /// <summary>
    /// Gets a value indicating whether the result represents a success.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the result represents a failure.
    /// </summary>
    public bool IsFailure { get; private set; }

    /// <summary>
    /// Gets the error associated with the result, if any.
    /// </summary>
    public AppError? Error { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// </summary>
    /// <param name="isSuccess">Indicates whether the operation was successful.</param>
    /// <param name="error">The error associated with the result, if any.</param>
    protected Result(bool isSuccess, AppError? error)
    {
        IsSuccess = isSuccess;
        IsFailure = !IsSuccess;
        Error = error;
    }

    /// <summary>
    /// Creates a <see cref="Result{T}"/> with a value. If the value is not null, it returns a successful result; otherwise, a failure.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value to be used in the result.</param>
    /// <returns>A <see cref="Result{T}"/> representing either success or failure.</returns>
    public static Result<TValue> Create<TValue>(TValue? value)
    {
        return value is not null
            ? Result<TValue>.Success(value)
            : Result<TValue>.Failure(new FailToCreateObjectError())!;
    }

    /// <summary>
    /// Asynchronously creates a <see cref="Result{T}"/> with a value. If the value is not null, it returns a successful result; otherwise, a failure.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">A task that represents the asynchronous operation returning the value.</param>
    /// <returns>A <see cref="Result{T}"/> representing either success or failure.</returns>
    public static async Task<Result<T>> Create<T>(Task<T> value)
    {
        var result = await value;
        return result is not null
            ? Result<T>.Success(result)
            : Result<T>.Failure(new FailToCreateObjectError())!;
    }

    /// <summary>
    /// Creates a successful result without a value.
    /// </summary>
    /// <returns>A <see cref="Result"/> instance representing success.</returns>
    public static Result Success() => new(true, null);

    /// <summary>
    /// Creates a failed result with an error.
    /// </summary>
    /// <param name="error">The error associated with the failure.</param>
    /// <returns>A <see cref="Result"/> instance representing failure.</returns>
    public static Result Failure(AppError? error) => new(false, error);

    /// <summary>
    /// Implicitly converts an <see cref="AppError"/> to a failed <see cref="Result"/>.
    /// </summary>
    /// <param name="error">The error to convert.</param>
    public static implicit operator Result(AppError? error) => Failure(error);

    /// <summary>
    /// Checks if the result's error matches the specified error type.
    /// </summary>
    /// <param name="errorType">The error type to check.</param>
    /// <returns><c>true</c> if the error matches the specified type; otherwise, <c>false</c>.</returns>
    public bool IsErrorType(ErrorType errorType) => Error?.ErrorType == errorType;

    /// <summary>
    /// Returns a string representation of the result.
    /// </summary>
    /// <returns>A string indicating success or failure and the associated error.</returns>
    public override string ToString() => IsSuccess ? "Success" : $"Failure: {Error}";
}

/// <summary>
/// Represents the result of an operation, including a value of type <typeparamref name="T"/> on success.
/// </summary>
/// <typeparam name="T">The type of the value associated with a successful result.</typeparam>
public class Result<T> : Result
{
    /// <summary>
    /// Gets the value associated with a successful result.
    /// </summary>
    public T Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{T}"/> class.
    /// </summary>
    /// <param name="value">The value of the result.</param>
    /// <param name="isSuccess">Indicates whether the operation was successful.</param>
    /// <param name="error">The error associated with the result, if any.</param>
    private Result(T value, bool isSuccess, AppError? error) : base(isSuccess, error) => Value = value;

    /// <summary>
    /// Creates a successful result with a value.
    /// </summary>
    /// <param name="value">The value of the result.</param>
    /// <returns>A <see cref="Result{T}"/> instance representing success.</returns>
    public static Result<T> Success(T value) => new(value, true, null);

    /// <summary>
    /// Creates a failed result with an error.
    /// </summary>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="error">The error associated with the failure.</param>
    /// <returns>A <see cref="Result{T}"/> instance representing failure.</returns>
    protected static Result<T?> Failure<TError>(TError error) where TError : AppError => new(default, false, error);

    /// <summary>
    /// Implicitly converts a value of type <typeparamref name="T"/> to a successful <see cref="Result{T}"/>.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    public static implicit operator Result<T>(T value) => Success(value);

    /// <summary>
    /// Implicitly converts an <see cref="AppError"/> to a failed <see cref="Result{T}"/>.
    /// </summary>
    /// <param name="error">The error to convert.</param>
    public static implicit operator Result<T?>(AppError? error) => Failure(error!)!;

    /// <summary>
    /// Returns a string representation of the result.
    /// </summary>
    /// <returns>A string indicating success with the value or failure with the associated error.</returns>
    public override string ToString() => IsSuccess ? $"Success: {Value}" : $"Failure: {Error}";

    /// <summary>
    /// Creates a failed <see cref="Result{T}"/> with the specified error.
    /// </summary>
    /// <param name="error">The error associated with the failure.</param>
    /// <returns>A <see cref="Result{T}"/> instance representing failure.</returns>
    public new static Result<T> Failure(AppError error) => new(default!, false, error);
}
