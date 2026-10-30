using E7.EasyResult.Errors;
using E7.EasyResult;

var checks = 0;
void Check(bool condition, string name)
{
    if (!condition) throw new InvalidOperationException(name);
    checks++;
}

Result<int> success = Result<int>.Success(42);
Check(success is Success<int> { Value: 42 }, "generic union across assembly");
Result unit = new Success();
Check(unit is Success, "non-generic union");
var error = new ConsumerError();
var payload = new Failure(error, [new Notification("a", "A"), new Notification("b", "B")]);
Result<int> failure = payload;
var called = false;
var mapped = failure.Map(value => { called = true; return value.ToString(); });
Check(mapped is Failure f && f.Error == error && f.Notifications.Count == 2 && !called,
    "entire failure propagated and callback skipped");
Result<AppError> errorAsValue = Result<AppError>.Success(error);
Check(errorAsValue is Success<AppError>, "error as success value disambiguated");
Result<AppError> errorAsFailure = payload;
Check(errorAsFailure is Failure, "custom error as failure");
Result<string?> nullSuccess = Result<string?>.Success(null);
Check(nullSuccess is Success<string?> { Value: null }, "explicit nullable success");
Result<string> uninitialized = default;
Check(!uninitialized.IsSuccess && !uninitialized.IsFailure, "default union has no case");
try
{
    uninitialized.Map(value => value.Length);
    throw new Exception("default guard missing");
}
catch (InvalidOperationException)
{
    checks++;
}

var source = new TaskCompletionSource<Result<int>>(TaskCreationOptions.RunContinuationsAsynchronously);
static async Task<Result<TOut>> MapAsync<TIn, TOut>(
    Task<Result<TIn>> task,
    Func<TIn, Task<TOut>> mapping)
{
    var result = await task.ConfigureAwait(false);
    return await result.Map(mapping).ConfigureAwait(false);
}

var pending = MapAsync(source.Task, value => Task.FromResult(value + 1));
Check(!pending.IsCompleted, "incomplete task returns control without blocking");
source.SetResult(success);
Check(await pending is Success<int> { Value: 43 }, "async success completes");
called = false;
var propagated = await MapAsync(Task.FromResult(failure), value =>
{
    called = true;
    return Task.FromResult(value.ToString());
});
Check(propagated is Failure af && af.Error == error && af.Notifications.Count == 2 && !called,
    "async failure preserves payload");
Check(typeof(Result<int>).IsValueType, "native union has value semantics");
Console.WriteLine($"Passed: {checks} union proof checks.");

public sealed record ConsumerError() : AppError("Consumer error", HttpErrorType.Business, "consumer.error")
{
    public override List<string> GetErrorsMessage() => [Message];
}
