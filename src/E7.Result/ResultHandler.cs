using E7.Result.Errors;

namespace E7.Result;

/// <summary>
/// Handles matching logic for results, allowing separation of concerns.
/// </summary>
public static class ResultHandler
{
    /// <summary>
    /// Matches the result to a corresponding action for success or failure.
    /// </summary>
    /// <typeparam name="T">The type of the result returned by the actions.</typeparam>
    /// <param name="result">The result to match.</param>
    /// <param name="onSuccess">The action to execute on success.</param>
    /// <param name="onFailure">The action to execute on failure.</param>
    /// <returns>The result of the executed action, either from <paramref name="onSuccess"/> or <paramref name="onFailure"/> based on the success status of the <paramref name="result"/>.</returns>
    public static T Match<T>(this Result result, Func<T> onSuccess, Func<AppError, T> onFailure)
        => result.IsSuccess ? onSuccess() : onFailure(result.Error!);
}