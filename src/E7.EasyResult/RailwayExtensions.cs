using E7.EasyResult.Errors;

namespace E7.EasyResult;

/// <summary>
/// Provides extension methods to apply Railway Oriented Programming (ROP) principles 
/// to <see cref="Result{T}"/> and <see cref="Result"/> objects. 
/// These extensions facilitate safe and expressive chaining of operations, 
/// error propagation, and conditional logic handling.
/// </summary>
public static class RailwayExtensions
{
    /// <summary>
    /// Validates the value of a successful result against a predicate. 
    /// If the predicate fails, returns a failure result with the specified error.
    /// Propagates failures without evaluation.
    /// </summary>
    public static Result<T> Ensure<T>(this Result<T> result, Func<T, bool> predicate, AppError error)
    {
        if (result.IsFailure) return result.Error!;
        return predicate(result.Value) ? result.Value! : Result<T>.Failure(error);
    }

    /// <summary>
    /// Validates the value of a task-wrapped result against a predicate asynchronously. 
    /// If the predicate fails, returns a failure result with the specified error.
    /// Propagates failures without evaluation.
    /// </summary>
    public static Result<T> Ensure<T>(this Task<Result<T>> result, Func<T, bool> predicate, AppError error)
    {
        if (result.Result.IsFailure) return result.Result;
        return predicate(result.Result.Value) ? result.Result : Result<T>.Failure(error)!;
    }

    /// <summary>
    /// Combines two successful results into a tuple. 
    /// If either result is a failure, returns the first encountered error.
    /// </summary>
    public static Result<(T1, T2)> Combine<T1, T2>(this Result<T1> result1, Result<T2> result2)
    {
        if (result1.IsFailure) return Result<(T1, T2)>.Failure(result1.Error!);

        return result2.IsFailure
            ? Result<(T1, T2)>.Failure(result2.Error!)
            : Result<(T1, T2)>.Success((result1.Value, result2.Value));
    }

    /// <summary>
    /// Executes a side-effect action if the result is successful.
    /// Returns the original result unmodified.
    /// </summary>
    public static Result<TIn> Tap<TIn>(this Result<TIn> result, Action<TIn> action)
    {
        if (result.IsSuccess) action(result.Value);
        return result;
    }

    /// <summary>
    /// Executes an asynchronous function if the result is successful.
    /// Does not modify or await the original result.
    /// </summary>
    public static async Task Tap<TIn>(this Result<TIn> result, Func<Task<TIn>> func)
    {
        if (result.IsSuccess) await func();
    }

    /// <summary>
    /// Executes an asynchronous action if the task-wrapped result is successful.
    /// Returns the original result.
    /// </summary>
    public static async Task<Result<TIn>> Tap<TIn>(this Task<Result<TIn>> resultTask, Func<TIn, Task> func)
    {
        var result = await resultTask;
        if (result.IsSuccess) await func(result.Value);
        return result;
    }

    /// <summary>
    /// Transforms the value of a successful result using the given mapping function.
    /// If the result is a failure, the error is propagated.
    /// </summary>
    public static Result<TOut> Map<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> mappingFunc)
    {
        return result.IsSuccess
            ? Result<TOut>.Success(mappingFunc(result.Value))
            : Result<TOut>.Failure(result.Error!);
    }

    /// <summary>
    /// Asynchronously transforms the value of a successful result using the given mapping function.
    /// If the result is a failure, the error is propagated.
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
    /// Applies a result-producing function to the value of a successful result.
    /// If the result is a failure, the error is propagated.
    /// </summary>
    public static Result<TOut> Bind<TIn, TOut>(this Result<TIn> result, Func<TIn, Result<TOut>> func)
    {
        return result.IsSuccess ? func(result.Value) : Result<TOut>.Failure(result.Error!);
    }

    /// <summary>
    /// Applies a result-producing function to the value of a task-wrapped result.
    /// If the result is a failure, the error is propagated.
    /// </summary>
    public static Result<TOut> Bind<TIn, TOut>(this Task<Result<TIn>> result, Func<TIn, Result<TOut>> func)
    {
        return result.Result.IsSuccess ? func(result.Result.Value) : Result<TOut>.Failure(result.Result.Error!);
    }

    /// <summary>
    /// Transforms the error of a failed result using the given mapping function.
    /// Successful results are preserved unchanged.
    /// </summary>
    public static Result<T> MapFailure<T>(this Result<T> result, Func<AppError, AppError> func)
    {
        return result.IsSuccess ? result.Value : Result<T>.Failure(func(result.Error!));
    }

    /// <summary>
    /// Executes a function on the value of a successful result and catches any exceptions,
    /// returning a failure result with the provided error if an exception occurs.
    /// </summary>
    public static Result<TOut?> TryCatch<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> func, AppError appError)
    {
        try
        {
            return (result.IsSuccess ? Result<TOut>.Success(func(result.Value)) : Result<TOut>.Failure(result.Error!))!;
        }
        catch
        {
            return Result<TOut>.Failure(appError)!;
        }
    }

    /// <summary>
    /// Matches a non-generic result, invoking the corresponding function based on success or failure.
    /// </summary>
    public static TOut Match<TOut>(
        this Result result,
        Func<TOut> onSuccess,
        Func<AppError, TOut> onFailure
    )
        => result.IsSuccess ? onSuccess() : onFailure(result.Error!);

    /// <summary>
    /// Matches a generic result, invoking the corresponding function based on success or failure.
    /// </summary>
    public static TOut Match<T, TOut>(
        this Result<T> result,
        Func<T, TOut> onSuccess,
        Func<AppError, TOut> onFailure
    )
        => result.IsSuccess ? onSuccess(result.Value) : onFailure(result.Error!);
}