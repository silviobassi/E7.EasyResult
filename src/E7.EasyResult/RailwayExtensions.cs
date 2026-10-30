using E7.EasyResult.Errors;

namespace E7.EasyResult;

/// <summary>Railway-oriented operations for result unions.</summary>
public static class RailwayExtensions
{
    /// <summary>Validates a successful result and returns a failure when the predicate is false.</summary>
    /// <typeparam name="T">The type of the successful value.</typeparam>
    /// <param name="result">The result to validate.</param>
    /// <param name="predicate">The validation predicate.</param>
    /// <param name="error">The error returned when validation fails.</param>
    public static Result<T> Ensure<T>(this Result<T> result, Func<T, bool> predicate, AppError error) =>
        result switch
        {
            Success<T> success => predicate(success.Value) ? result : Result<T>.Failure(error),
            Failure failure => new Failure(failure.Error, [.. failure.Notifications]),
            _ => throw Result.InvalidState()
        };

    /// <summary>Validates a successful result and returns a notification failure when the predicate is false.</summary>
    /// <typeparam name="T">The type of the successful value.</typeparam>
    /// <param name="result">The result to validate.</param>
    /// <param name="predicate">The validation predicate.</param>
    /// <param name="notification">The notification returned when validation fails.</param>
    public static Result<T> Ensure<T>(this Result<T> result, Func<T, bool> predicate, Notification notification) =>
        result switch
        {
            Success<T> success => predicate(success.Value) ? result : Result<T>.Failure([notification]),
            Failure failure => new Failure(failure.Error, [.. failure.Notifications]),
            _ => throw Result.InvalidState()
        };

    /// <summary>Asynchronously validates a result after awaiting its task.</summary>
    /// <typeparam name="T">The type of the successful value.</typeparam>
    /// <param name="task">The task producing the result.</param>
    /// <param name="predicate">The validation predicate.</param>
    /// <param name="error">The error returned when validation fails.</param>
    public static async Task<Result<T>> Ensure<T>(
        this Task<Result<T>> task,
        Func<T, bool> predicate,
        AppError error)
    {
        var result = await task.ConfigureAwait(false);
        return result.Ensure(predicate, error);
    }

    /// <summary>Combines two results, preserving values on success and all failure information otherwise.</summary>
    /// <typeparam name="T1">The type of the first successful value.</typeparam>
    /// <typeparam name="T2">The type of the second successful value.</typeparam>
    /// <param name="first">The first result.</param>
    /// <param name="second">The second result.</param>
    /// <returns>A tuple containing both values, or a failure with concatenated notifications.</returns>
    public static Result<(T1, T2)> Combine<T1, T2>(
        this Result<T1> first,
        Result<T2> second)
    {
        if (first is Success<T1> firstSuccess && second is Success<T2> secondSuccess)
            return Result<(T1, T2)>.Success((firstSuccess.Value, secondSuccess.Value));

        Notification[] notifications = [.. first.Notifications, .. second.Notifications];
        return Result<(T1, T2)>.Failure(first.Error ?? second.Error, notifications);
    }

    /// <summary>Executes a synchronous side effect for a successful result.</summary>
    /// <typeparam name="T">The type of the successful value.</typeparam>
    /// <param name="result">The result whose value is observed.</param>
    /// <param name="action">The side effect to execute on success.</param>
    public static Result<T> Tap<T>(this Result<T> result, Action<T> action)
    {
        if (result is Success<T> success)
            action(success.Value);
        else if (result is not Failure)
            throw Result.InvalidState();

        return result;
    }

    /// <summary>Executes an asynchronous side effect for a successful result.</summary>
    /// <typeparam name="T">The type of the successful value.</typeparam>
    /// <param name="result">The result whose value is observed.</param>
    /// <param name="action">The asynchronous side effect to execute on success.</param>
    public static async Task Tap<T>(this Result<T> result, Func<Task<T>> action)
    {
        if (result is Success<T>)
            await action().ConfigureAwait(false);
        else if (result is not Failure)
            throw Result.InvalidState();
    }

    /// <summary>Awaits a result and executes an asynchronous side effect on success.</summary>
    /// <typeparam name="T">The type of the successful value.</typeparam>
    /// <param name="task">The task producing the result.</param>
    /// <param name="action">The asynchronous side effect to execute on success.</param>
    /// <returns>The original result after the side effect completes.</returns>
    public static async Task<Result<T>> Tap<T>(
        this Task<Result<T>> task,
        Func<T, Task> action)
    {
        var result = await task.ConfigureAwait(false);
        if (result is Success<T> success)
            await action(success.Value).ConfigureAwait(false);
        else if (result is not Failure)
            throw Result.InvalidState();

        return result;
    }

    /// <summary>Transforms the value of a successful result.</summary>
    /// <typeparam name="TIn">The input value type.</typeparam>
    /// <typeparam name="TOut">The output value type.</typeparam>
    /// <param name="result">The result to transform.</param>
    /// <param name="mapping">The value transformation.</param>
    public static Result<TOut> Map<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, TOut> mapping)
    {
        return result switch
        {
            Success<TIn> success => Result<TOut>.Success(mapping(success.Value)),
            Failure failure => new Failure(failure.Error, [.. failure.Notifications]),
            _ => throw Result.InvalidState()
        };
    }

    /// <summary>Asynchronously transforms the value of a successful result.</summary>
    /// <typeparam name="TIn">The input value type.</typeparam>
    /// <typeparam name="TOut">The output value type.</typeparam>
    /// <param name="result">The result to transform.</param>
    /// <param name="mapping">The asynchronous value transformation.</param>
    public static async Task<Result<TOut>> Map<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, Task<TOut>> mapping)
    {
        return result switch
        {
            Success<TIn> success => Result<TOut>.Success(
                await mapping(success.Value).ConfigureAwait(false)),
            Failure failure => new Failure(failure.Error, [.. failure.Notifications]),
            _ => throw Result.InvalidState()
        };
    }

    /// <summary>Chains a synchronous result-producing operation after success.</summary>
    /// <typeparam name="TIn">The input value type.</typeparam>
    /// <typeparam name="TOut">The output value type.</typeparam>
    /// <param name="result">The result to continue.</param>
    /// <param name="function">The next operation.</param>
    public static Result<TOut> Bind<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, Result<TOut>> function) =>
        result switch
        {
            Success<TIn> success => function(success.Value),
            Failure failure => new Failure(failure.Error, [.. failure.Notifications]),
            _ => throw Result.InvalidState()
        };

    /// <summary>Chains an asynchronous result-producing operation after success.</summary>
    /// <typeparam name="TIn">The input value type.</typeparam>
    /// <typeparam name="TOut">The output value type.</typeparam>
    /// <param name="result">The result to continue.</param>
    /// <param name="function">The asynchronous next operation.</param>
    public static async Task<Result<TOut>> BindAsync<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, Task<Result<TOut>>> function) =>
        result switch
        {
            Success<TIn> success => await function(success.Value).ConfigureAwait(false),
            Failure failure => new Failure(failure.Error, [.. failure.Notifications]),
            _ => throw Result.InvalidState()
        };

    /// <summary>Wraps a synchronous bind operation in a task.</summary>
    /// <typeparam name="TIn">The input value type.</typeparam>
    /// <typeparam name="TOut">The output value type.</typeparam>
    /// <param name="result">The result to continue.</param>
    /// <param name="function">The next operation.</param>
    public static Task<Result<TOut>> BindAsync<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, Result<TOut>> function) =>
        Task.FromResult(result.Bind(function));

    /// <summary>Awaits a result and chains a synchronous result-producing operation.</summary>
    /// <typeparam name="TIn">The input value type.</typeparam>
    /// <typeparam name="TOut">The output value type.</typeparam>
    /// <param name="task">The task producing the result.</param>
    /// <param name="function">The next operation.</param>
    public static async Task<Result<TOut>> Bind<TIn, TOut>(
        this Task<Result<TIn>> task,
        Func<TIn, Result<TOut>> function)
    {
        var result = await task.ConfigureAwait(false);
        return result.Bind(function);
    }

    /// <summary>Awaits a result and chains an asynchronous result-producing operation.</summary>
    /// <typeparam name="TIn">The input value type.</typeparam>
    /// <typeparam name="TOut">The output value type.</typeparam>
    /// <param name="task">The task producing the result.</param>
    /// <param name="function">The asynchronous next operation.</param>
    public static async Task<Result<TOut>> BindAsync<TIn, TOut>(
        this Task<Result<TIn>> task,
        Func<TIn, Task<Result<TOut>>> function)
    {
        var result = await task.ConfigureAwait(false);
        return await result.BindAsync(function).ConfigureAwait(false);
    }

    /// <summary>Awaits a result and chains a synchronous operation without blocking the caller.</summary>
    /// <typeparam name="TIn">The input value type.</typeparam>
    /// <typeparam name="TOut">The output value type.</typeparam>
    /// <param name="task">The task producing the result.</param>
    /// <param name="function">The next operation.</param>
    public static async Task<Result<TOut>> BindAsync<TIn, TOut>(
        this Task<Result<TIn>> task,
        Func<TIn, Result<TOut>> function)
    {
        var result = await task.ConfigureAwait(false);
        return result.Bind(function);
    }

    /// <summary>Transforms the primary error while preserving notifications.</summary>
    /// <typeparam name="T">The type of the successful value.</typeparam>
    /// <param name="result">The result whose error is transformed.</param>
    /// <param name="function">The error transformation.</param>
    public static Result<T> MapFailure<T>(
        this Result<T> result,
        Func<AppError, AppError> function) =>
        result switch
        {
            Success<T> => result,
            Failure failure => new Failure(
                failure.Error is null ? null : function(failure.Error),
                [.. failure.Notifications]),
            _ => throw Result.InvalidState()
        };

    /// <summary>Runs a transformation and converts exceptions into the supplied application error.</summary>
    /// <typeparam name="T">The input value type.</typeparam>
    /// <typeparam name="TOut">The output value type.</typeparam>
    /// <param name="result">The result to transform.</param>
    /// <param name="function">The transformation that may throw.</param>
    /// <param name="appError">The error used when the transformation throws.</param>
    public static Result<TOut> TryCatch<T, TOut>(
        this Result<T> result,
        Func<T, TOut> function,
        AppError appError)
    {
        try
        {
            return result switch
            {
                Success<T> success => Result<TOut>.Success(function(success.Value)),
                Failure failure => new Failure(failure.Error, [.. failure.Notifications]),
                _ => throw Result.InvalidState()
            };
        }
        catch
        {
            return Result<TOut>.Failure(appError);
        }
    }

    /// <summary>Selects the success or failure branch of a non-generic result.</summary>
    /// <typeparam name="TOut">The return type of both callbacks.</typeparam>
    /// <param name="result">The result to inspect.</param>
    /// <param name="onSuccess">The callback for success.</param>
    /// <param name="onFailure">The callback for failure; its argument may be <see langword="null"/>.</param>
    public static TOut Match<TOut>(
        this Result result,
        Func<TOut> onSuccess,
        Func<AppError?, TOut> onFailure) =>
        result switch
        {
            Success => onSuccess(),
            Failure failure => onFailure(failure.Error),
            _ => throw Result.InvalidState()
        };

    /// <summary>Selects the success or failure branch and returns an object.</summary>
    /// <typeparam name="T">The type of the successful value.</typeparam>
    /// <param name="result">The result to inspect.</param>
    /// <param name="onSuccess">The callback for success.</param>
    /// <param name="onFailure">The callback for failure; its argument may be <see langword="null"/>.</param>
    public static object Match<T>(
        this Result<T> result,
        Func<T, object> onSuccess,
        Func<AppError?, object> onFailure) =>
        result switch
        {
            Success<T> success => onSuccess(success.Value),
            Failure failure => onFailure(failure.Error),
            _ => throw Result.InvalidState()
        };

    /// <summary>Selects the success or failure branch of a value-bearing result.</summary>
    /// <typeparam name="T">The type of the successful value.</typeparam>
    /// <typeparam name="TOut">The return type of both callbacks.</typeparam>
    /// <param name="result">The result to inspect.</param>
    /// <param name="onSuccess">The callback for success.</param>
    /// <param name="onFailure">The callback for failure; its argument may be <see langword="null"/>.</param>
    public static TOut Match<T, TOut>(
        this Result<T> result,
        Func<T, TOut> onSuccess,
        Func<AppError?, TOut> onFailure) =>
        result switch
        {
            Success<T> success => onSuccess(success.Value),
            Failure failure => onFailure(failure.Error),
            _ => throw Result.InvalidState()
        };
}
