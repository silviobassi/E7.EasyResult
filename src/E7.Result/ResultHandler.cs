using E7.Result.Errors;

namespace E7.Result;

/// <summary>
/// Handles matching logic for results, allowing separation of concerns.
/// </summary>
public static class ResultHandler
{
    /// <summary>
    /// Matches the resultS to a corresponding action for success or failure.
    /// </summary>
    /// <typeparam name="TResult">The type of the resultS returned by the actions.</typeparam>
    /// <param name="resultS">The resultS to match.</param>
    /// <param name="onSuccess">The action to execute on success.</param>
    /// <param name="onFailure">The action to execute on failure.</param>
    /// <returns>The resultS of the executed action, either from <paramref name="onSuccess"/> or <paramref name="onFailure"/> based on the success status of the <paramref name="resultS"/>.</returns>
    public static TResult Match<TResult>(this ResultS resultS, Func<TResult> onSuccess, Func<AppError, TResult> onFailure)
        => resultS.IsSuccess ? onSuccess() : onFailure(resultS.Error!);
}