using E7.Result.Errors;
using Microsoft.AspNetCore.Mvc;

namespace E7.Result;

/// <summary>
/// Handles matching logic for results, allowing separation of concerns.
/// </summary>
internal static class ResultHandler
{
    /// <summary>
    /// Matches the result to a corresponding action for success or failure.
    /// </summary>
    /// <param name="result">The result to match.</param>
    /// <param name="onSuccess">The action to execute on success.</param>
    /// <param name="onFailure">The action to execute on failure.</param>
    /// <returns>An <see cref="IActionResult"/> representing the result of the match.</returns>
    public static IActionResult Match(
        this Result result, Func<IActionResult> onSuccess,
        Func<AppError, IActionResult> onFailure
    )
        => result.IsSuccess ? onSuccess() : onFailure(result.Error!);
}