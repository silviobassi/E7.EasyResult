using E7.Result.Errors;

namespace E7.Result;

/// <summary>
/// Represents the resultS of an operation, indicating success or failure.
/// </summary>
public class ResultS
{
    /// <summary>
    /// Gets a value indicating whether the resultS represents a success.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the resultS represents a failure.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the error associated with the resultS, if any.
    /// </summary>
    public AppError? Error { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResultS"/> class.
    /// </summary>
    /// <param name="isSuccess">Indicates whether the operation was successful.</param>
    /// <param name="error">The error associated with the resultS, if any.</param>
    protected ResultS(bool isSuccess, AppError? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>
    /// Creates a successful resultS.
    /// </summary>
    /// <returns>A <see cref="ResultS"/> instance representing success.</returns>
    public static ResultS Success() => new(true, null);

    /// <summary>
    /// Creates a failed resultS.
    /// </summary>
    /// <param name="error">The error associated with the failure.</param>
    /// <returns>A <see cref="ResultS"/> instance representing failure.</returns>
    public static ResultS Failure(AppError? error) => new(false, error);

    /// <summary>
    /// Implicitly converts an <see cref="AppError"/> to a failed <see cref="ResultS"/>.
    /// </summary>
    /// <param name="error">The error to convert.</param>
    public static implicit operator ResultS(AppError error) => Failure(error);

    /// <summary>
    /// Checks if the resultS's error matches the specified error type.
    /// </summary>
    /// <param name="errorType">The error type to check.</param>
    /// <returns><c>true</c> if the error matches the specified type; otherwise, <c>false</c>.</returns>
    public bool IsErrorType(ErrorType errorType) => Error?.ErrorType == errorType;

    /// <summary>
    /// Returns a string representation of the resultS.
    /// </summary>
    /// <returns>A string indicating success or failure and the associated error.</returns>
    public override string ToString() => IsSuccess ? "Success" : $"Failure: {Error}";
}

/// <summary>
/// Represents the resultS of an operation, including a value of type <typeparamref name="T"/> on success.
/// </summary>
/// <typeparam name="T">The type of the value associated with a successful resultS.</typeparam>
public class ResultS<T> : ResultS
{
    /// <summary>
    /// Gets the value associated with a successful resultS.
    /// </summary>
    public T Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResultSS{T}"/> class.
    /// </summary>
    /// <param name="value">The value of the resultS.</param>
    /// <param name="isSuccess">Indicates whether the operation was successful.</param>
    /// <param name="error">The error associated with the resultS, if any.</param>
    private ResultS(T value, bool isSuccess, AppError? error) : base(isSuccess, error) => Value = value;

    /// <summary>
    /// Creates a successful resultS with a value.
    /// </summary>
    /// <param name="value">The value of the resultS.</param>
    /// <returns>A <see cref="ResultSS{T}"/> instance representing success.</returns>
    public static ResultS<T> Success(T value) => new(value, true, null);

    /// <summary>
    /// Creates a failed resultS.
    /// </summary>
    /// <param name="error">The error associated with the failure.</param>
    /// <returns>A <see cref="ResultSS{T}"/> instance representing failure.</returns>
    public new static ResultS<T?> Failure(AppError? error) => new(default, false, error);

    /// <summary>
    /// Implicitly converts a value of type <typeparamref name="T"/> to a successful <see cref="ResultSS{T}"/>.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    public static implicit operator ResultS<T>(T value) => Success(value);

    /// <summary>
    /// Implicitly converts an <see cref="AppError"/> to a failed <see cref="ResultSS{T}"/>.
    /// </summary>
    /// <param name="error">The error to convert.</param>
    public static implicit operator ResultS<T?>(AppError error) => Failure(error);

    /// <summary>
    /// Returns a string representation of the resultS.
    /// </summary>
    /// <returns>A string indicating success with the value or failure with the associated error.</returns>
    public override string ToString() => IsSuccess ? $"Success: {Value}" : $"Failure: {Error}";
}
