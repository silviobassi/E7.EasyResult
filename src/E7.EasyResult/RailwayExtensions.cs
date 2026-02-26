using E7.EasyResult.Errors;

namespace E7.EasyResult;

/// <summary>
/// Provides extension methods to apply Railway Oriented Programming (ROP) principles 
/// to <see cref="Result{T}"/> and <see cref="Result"/> objects. 
/// These extensions facilitate safe and expressive chaining of operations, 
/// error propagation, and conditional logic handling.
/// </summary>
/// <author>Silvio Luiz Bassi</author>
/// <company>Enfatiza7 Consultoria em Tecnologia LTDA</company>
public static class RailwayExtensions
{
    extension<TIn>(Result<TIn> result)
    {
        /// <summary>
        /// Safely propagates a failure state, including the primary error and all accumulated notifications,
        /// to a new generic result type.
        /// </summary>
        /// <typeparam name="TOut">The target type of the new result.</typeparam>
        /// <returns>A new <see cref="Result{TOut}"/> containing the original errors and notifications.</returns>
        private Result<TOut> PropagateFailure<TOut>()
        {
            return Result<TOut>.Failure(result.Error, result.Notifications.ToList());
        }

        /// <summary>
        /// Validates the value of a successful result against a specified predicate. 
        /// If the predicate evaluates to false, returns a failure result containing the provided <see cref="AppError"/>.
        /// Existing failures are propagated without evaluating the predicate.
        /// </summary>
        /// <param name="predicate">The condition to evaluate the result's value against.</param>
        /// <param name="error">The error to return if the predicate fails.</param>
        /// <returns>The original result if successful and the predicate passes; otherwise, a failure result.</returns>
        public Result<TIn> Ensure(Func<TIn, bool> predicate, AppError error)
        {
            if (result.IsFailure) return result.Error!;
            return predicate(result.Value) ? result.Value! : Result<TIn>.Failure(error);
        }

        /// <summary>
        /// Validates the value of a successful result against a specified predicate. 
        /// If the predicate evaluates to false, appends the provided <see cref="Notification"/> to the failure state.
        /// Existing failures are propagated without evaluating the predicate.
        /// </summary>
        /// <param name="predicate">The condition to evaluate the result's value against.</param>
        /// <param name="notification">The notification to append if the predicate fails.</param>
        /// <returns>The original result if successful and the predicate passes; otherwise, a failure result with the notification.</returns>
        public Result<TIn> Ensure(Func<TIn, bool> predicate, Notification notification)
        {
            if (result.IsFailure) return result;
            return predicate(result.Value) ? result : Result<TIn>.Failure([notification]);
        }
    }

    /// <summary>
    /// Validates the value of a task-wrapped result against a predicate asynchronously. 
    /// If the predicate fails, returns a failure result with the specified error.
    /// Propagates failures without evaluation.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="result">The task-wrapped result to evaluate.</param>
    /// <param name="predicate">The condition to evaluate the result's value against.</param>
    /// <param name="error">The error to return if the predicate fails.</param>
    /// <returns>The original result if successful and the predicate passes; otherwise, a failure result.</returns>
    public static Result<T> Ensure<T>(this Task<Result<T>> result, Func<T, bool> predicate, AppError error)
    {
        if (result.Result.IsFailure) return result.Result;
        return predicate(result.Result.Value) ? result.Result : Result<T>.Failure(error);
    }

    extension<T1>(Result<T1> result1)
    {
        /// <summary>
        /// Combines the current result with another result into a tuple. 
        /// If either result is a failure, it aggregates all notifications from both results and returns the primary error.
        /// </summary>
        /// <typeparam name="T2">The type of the second result's value.</typeparam>
        /// <param name="result2">The second result to combine with the current one.</param>
        /// <returns>A successful result containing a tuple of both values, or an aggregated failure result.</returns>
        public Result<(T1, T2)> Combine<T2>(Result<T2> result2)
        {
            if (result1.IsSuccess && result2.IsSuccess)
                return Result<(T1, T2)>.Success((result1.Value, result2.Value));

            var combinedNotifications = result1.Notifications
                .Concat(result2.Notifications)
                .ToList();

            var primaryError = result1.Error ?? result2.Error;

            return Result<(T1, T2)>.Failure(primaryError, combinedNotifications);
        }

        /// <summary>
        /// Executes a synchronous side effect action if the result is successful.
        /// The original result is returned unmodified, allowing for method chaining.
        /// </summary>
        /// <param name="action">The action to execute using the result's value.</param>
        /// <returns>The original <see cref="Result{T1}"/>.</returns>
        public Result<T1> Tap(Action<T1> action)
        {
            if (result1.IsSuccess) action(result1.Value);
            return result1;
        }

        /// <summary>
        /// Executes an asynchronous side effect action if the result is successful.
        /// The original result state is not modified.
        /// </summary>
        /// <param name="func">The asynchronous function to execute.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task Tap(Func<Task<T1>> func)
        {
            if (result1.IsSuccess) await func();
        }
    }

    /// <summary>
    /// Executes an asynchronous side effect action if the task-wrapped result is successful.
    /// Returns the original result unmodified to allow further chaining.
    /// </summary>
    /// <typeparam name="TIn">The type of the result value.</typeparam>
    /// <param name="resultTask">The task-wrapped result to evaluate.</param>
    /// <param name="func">The asynchronous action to execute using the result's value.</param>
    /// <returns>A <see cref="Task"/> resolving to the original <see cref="Result{TIn}"/>.</returns>
    public static async Task<Result<TIn>> Tap<TIn>(this Task<Result<TIn>> resultTask, Func<TIn, Task> func)
    {
        var result = await resultTask;
        if (result.IsSuccess) await func(result.Value);
        return result;
    }

    extension<TIn>(Result<TIn> result)
    {
        /// <summary>
        /// Transforms the value of a successful result using the provided mapping function.
        /// If the result is a failure, both the primary error and notifications are safely propagated.
        /// </summary>
        /// <typeparam name="TOut">The type returned by the mapping function.</typeparam>
        /// <param name="mappingFunc">The function to transform the value.</param>
        /// <returns>A new <see cref="Result{TOut}"/> containing the transformed value or the propagated failure.</returns>
        public Result<TOut> Map<TOut>(Func<TIn, TOut> mappingFunc)
        {
            return result.IsSuccess
                ? Result<TOut>.Success(mappingFunc(result.Value))
                : Result<TOut>.Failure(result.Error, result.Notifications.ToList());
        }

        /// <summary>
        /// Asynchronously transforms the value of a successful result using the provided mapping function.
        /// If the result is a failure, the error is propagated.
        /// </summary>
        /// <typeparam name="TOut">The type returned by the asynchronous mapping function.</typeparam>
        /// <param name="mapFunction">The asynchronous function to transform the value.</param>
        /// <returns>A <see cref="Task"/> resolving to a new <see cref="Result{TOut}"/>.</returns>
        public async Task<Result<TOut>> Map<TOut>(Func<TIn, Task<TOut>> mapFunction)
        {
            if (result.IsSuccess) return await mapFunction(result.Value);
            return Result<TOut>.Failure(result.Error!);
        }

        /// <summary>
        /// Chains another operation that returns a <see cref="Result{TOut}"/> if the current result is successful.
        /// If the current result is a failure, its errors are propagated without invoking the function.
        /// </summary>
        /// <typeparam name="TOut">The value type of the returned result.</typeparam>
        /// <param name="func">The function that returns the next <see cref="Result{TOut}"/> in the chain.</param>
        /// <returns>The result of the chained operation or the propagated failure.</returns>
        public Result<TOut> Bind<TOut>(Func<TIn, Result<TOut>> func)
        {
            return result.IsSuccess ? func(result.Value) : Result<TOut>.Failure(result.Error!);
        }
    }

    /// <summary>
    /// Chains another operation that returns a <see cref="Result{TOut}"/> to a task-wrapped successful result.
    /// If the result is a failure, the error is propagated.
    /// </summary>
    /// <typeparam name="TIn">The type of the current result value.</typeparam>
    /// <typeparam name="TOut">The type of the returned result value.</typeparam>
    /// <param name="result">The task-wrapped result to evaluate.</param>
    /// <param name="func">The function that returns the next <see cref="Result{TOut}"/> in the chain.</param>
    /// <returns>The result of the chained operation or the propagated failure.</returns>
    public static Result<TOut> Bind<TIn, TOut>(this Task<Result<TIn>> result, Func<TIn, Result<TOut>> func)
    {
        return result.Result.IsSuccess ? func(result.Result.Value) : Result<TOut>.Failure(result.Result.Error!);
    }

    extension<T>(Result<T> result)
    {
        /// <summary>
        /// Transforms the primary <see cref="AppError"/> of a failed result using the provided mapping function.
        /// Successful results are returned unmodified.
        /// </summary>
        /// <param name="func">The function to transform the primary error.</param>
        /// <returns>A <see cref="Result{T}"/> with the transformed error, or the original successful result.</returns>
        public Result<T> MapFailure(Func<AppError, AppError> func)
        {
            return result.IsSuccess ? result.Value : Result<T>.Failure(func(result.Error!));
        }

        /// <summary>
        /// Executes a function on the value of a successful result and catches any exceptions thrown during execution.
        /// If an exception occurs, it returns a failure result with the provided <see cref="AppError"/>.
        /// </summary>
        /// <typeparam name="TOut">The return type of the function.</typeparam>
        /// <param name="func">The function to execute.</param>
        /// <param name="appError">The error to return if an exception is caught.</param>
        /// <returns>A successful <see cref="Result{TOut}"/>, or a failure result if an exception is caught or if the original result was a failure.</returns>
        public Result<TOut?> TryCatch<TOut>(Func<T, TOut> func, AppError appError)
        {
            try
            {
                return (result.IsSuccess
                    ? Result<TOut>.Success(func(result.Value))
                    : Result<TOut>.Failure(result.Error!))!;
            }
            catch
            {
                return Result<TOut>.Failure(appError)!;
            }
        }
    }

    /// <summary>
    /// Evaluates a non-generic result and invokes the corresponding function based on its success or failure state.
    /// </summary>
    /// <typeparam name="TOut">The return type of the match functions.</typeparam>
    /// <param name="result">The result to evaluate.</param>
    /// <param name="onSuccess">The function to execute if the result is successful.</param>
    /// <param name="onFailure">The function to execute if the result is a failure. Receives the original error.</param>
    /// <returns>The value returned by the executed function.</returns>
    public static TOut Match<TOut>(
        this Result result,
        Func<TOut> onSuccess,
        Func<AppError, TOut> onFailure
    )
        => result.IsSuccess ? onSuccess() : onFailure(result.Error!);

    /// <summary>
    /// Evaluates a generic result and invokes the corresponding function based on its success or failure state.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <typeparam name="TOut">The return type of the match functions.</typeparam>
    /// <param name="result">The result to evaluate.</param>
    /// <param name="onSuccess">The function to execute if the result is successful. Receives the result's value.</param>
    /// <param name="onFailure">The function to execute if the result is a failure. Receives the original error.</param>
    /// <returns>The value returned by the executed function.</returns>
    public static TOut Match<T, TOut>(
        this Result<T> result,
        Func<T, TOut> onSuccess,
        Func<AppError, TOut> onFailure
    )
        => result.IsSuccess ? onSuccess(result.Value) : onFailure(result.Error!);
}