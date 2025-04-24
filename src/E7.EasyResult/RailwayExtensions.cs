using E7.EasyResult.Errors;

namespace E7.EasyResult;

/// <summary>
/// Provides extension methods to compose operations using the Railway Oriented Programming pattern.
/// Enables chaining, transformation, and validation of <see cref="Result{T}"/> instances in a fluent and safe manner.
/// </summary>
public static class RailwayExtensions
{
    /// <summary>
    /// Ensures that a result's value satisfies a given predicate. Returns a failure with the specified error if the predicate fails.
    /// </summary>
    public static Result<T> Ensure<T>(this Result<T> result, Func<T, bool> predicate, AppError error)
    {
        if (result.IsFailure) return result.Error!;
        return predicate(result.Value) ? result.Value! : Result<T>.Failure(error);
    }

    /// <summary>
    /// Asynchronously ensures that a result's value satisfies a predicate. Returns a failure if the predicate fails.
    /// </summary>
    public static Result<T> Ensure<T>(this Task<Result<T>> result, Func<T, bool> predicate, AppError error)
    {
        if (result.Result.IsFailure) return result.Result;
        return predicate(result.Result.Value) ? result.Result : Result<T>.Failure(error)!;
    }

    /// <summary>
    /// Combines two successful results into a single tuple. Returns the first failure encountered.
    /// </summary>
    public static Result<(T1, T2)> Combine<T1, T2>(this Result<T1> result1, Result<T2> result2)
    {
        if (result1.IsFailure) return Result<(T1, T2)>.Failure(result1.Error!);

        return result2.IsFailure
            ? Result<(T1, T2)>.Failure(result2.Error!)
            : Result<(T1, T2)>.Success((result1.Value, result2.Value));
    }

    /// <summary>
    /// Executes an action when the result is successful. Returns the original result.
    /// </summary>
    public static Result<TIn> Tap<TIn>(this Result<TIn> result, Action<TIn> action)
    {
        if (result.IsSuccess) action(result.Value);
        return result;
    }

    /// <summary>
    /// Executes an asynchronous function when the result is successful. Returns the original result.
    /// </summary>
    public static async Task Tap<TIn>(this Result<TIn> result, Func<Task<TIn>> func)
    {
        if (result.IsSuccess) await func();
    }

    /// <summary>
    /// Executes an asynchronous action using the result's value when successful. Returns the original result.
    /// </summary>
    public static async Task<Result<TIn>> Tap<TIn>(this Task<Result<TIn>> resultTask, Func<TIn, Task> func)
    {
        var result = await resultTask;
        if (result.IsSuccess) await func(result.Value);
        return result;
    }

    /// <summary>
    /// Maps a successful result's value to another type. Preserves failure.
    /// </summary>
    public static Result<TOut> Map<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> mappingFunc)
    {
        if (result.IsSuccess) return Result<TOut>.Success(mappingFunc(result.Value));
        return Result<TOut>.Failure(result.Error!);
    }

    /// <summary>
    /// Asynchronously maps a result's value to another type. Preserves failure.
    /// </summary>
    public static async Task<Result<TOut>> Map<TIn, TOut>(
        this Result<TIn> taskResult,
        Func<TIn, Task<TOut>> mapFunction
    )
    {
        if (taskResult.IsSuccess) return await mapFunction(taskResult.Value);
        return Result<TOut>.Failure(taskResult.Error!);
    }

    /// <summary>
    /// Chains another result-producing function if the result is successful. Preserves failure.
    /// </summary>
    public static Result<TOut> Bind<TIn, TOut>(this Result<TIn> result, Func<TIn, Result<TOut>> func)
    {
        return !result.IsSuccess ? Result<TOut>.Failure(result.Error!) : func(result.Value);
    }

    /// <summary>
    /// Chains another result-producing function from an asynchronous result. Preserves failure.
    /// </summary>
    public static Result<TOut> Bind<TIn, TOut>(this Task<Result<TIn>> result, Func<TIn, Result<TOut>> func)
    {
        return !result.Result.IsSuccess ? Result<TOut>.Failure(result.Result.Error!) : func(result.Result.Value);
    }

    /// <summary>
    /// Maps the error of a failed result to another error type. Preserves success.
    /// </summary>
    public static Result<T> MapFailure<T>(this Result<T> result, Func<AppError, AppError> func)
    {
        return result.IsSuccess ? result.Value : Result<T>.Failure(func(result.Error!));
    }

    /// <summary>
    /// Attempts to run a function on a successful result. Returns a failure if the function throws.
    /// </summary>
    public static Result<TOut?> TryCatch<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> func, AppError appError)
    {
        try
        {
            return result.IsSuccess ? Result<TOut>.Success(func(result.Value)) : Result<TOut>.Failure(result.Error);
        }
        catch
        {
            return Result<TOut>.Failure(appError)!;
        }
    }

    /// <summary>
    /// Pattern matches a result. Invokes the success or failure handler accordingly.
    /// </summary>
    public static TOut Match<TOut>(
        this Result result,
        Func<TOut> onSuccess,
        Func<AppError, TOut> onFailure
    )
        => result.IsSuccess ? onSuccess() : onFailure(result.Error!);

    /// <summary>
    /// Pattern matches a typed result. Invokes the appropriate function depending on success or failure.
    /// </summary>
    public static TOut Match<T, TOut>(
        this Result<T> result,
        Func<T, TOut> onSuccess,
        Func<AppError, TOut> onFailure
    )
        => result.IsSuccess ? onSuccess(result.Value) : onFailure(result.Error!);
}