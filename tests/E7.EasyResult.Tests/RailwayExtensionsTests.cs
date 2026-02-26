using E7.EasyResult.Errors;
using Shouldly;

namespace E7.EasyResult.Tests;

public sealed class RailwayExtensionsTests
{
    // 1. Converted to a record, following AppError
    private record DummyError() : AppError("Object not found.", HttpErrorType.NotFoundRule, nameof(DummyError))
    {
        public override List<string> GetErrorsMessage() => [Message];
    }

    [Fact]
    public void Ensure_Should_Return_Same_Result_When_Predicate_Is_True()
    {
        var result = Result<int>.Success(10);
        var ensured = result.Ensure(x => x > 5, new DummyError());

        ensured.IsSuccess.ShouldBeTrue();
        ensured.Value.ShouldBe(10);
    }

    [Fact]
    public void Ensure_Should_Return_Error_When_Predicate_Is_False()
    {
        var result = Result<int>.Success(3);
        var error = new DummyError();
        var ensured = result.Ensure(x => x > 5, error);

        ensured.IsFailure.ShouldBeTrue();
        ensured.Error.ShouldBe(error);
    }

    // NEW TEST: Ensure that Ensure works with Notifications
    [Fact]
    public void Ensure_Should_Return_Notification_When_Predicate_Is_False()
    {
        var result = Result<int>.Success(3);
        var notification = new Notification("01", "Value too low");

        var ensured = result.Ensure(x => x > 5, notification);

        ensured.IsFailure.ShouldBeTrue();
        ensured.Notifications.ShouldContain(notification);
    }

    [Fact]
    public void Ensure_Async_Should_Return_Same_Result_When_Predicate_Is_True()
    {
        var resultTask = Task.FromResult(Result<string>.Success("ok"));

        // Removed 'await', as Ensure here resolves the task and returns the Result directly
        var ensured = resultTask.Ensure(x => x == "ok", new DummyError());

        ensured.IsSuccess.ShouldBeTrue();
        ensured.Value.ShouldBe("ok");
    }

    [Fact]
    public void Combine_Should_Return_Success_When_Both_Are_Success()
    {
        var r1 = Result<int>.Success(1);
        var r2 = Result<string>.Success("two");

        var combined = r1.Combine(r2);

        combined.IsSuccess.ShouldBeTrue();
        combined.Value.ShouldBe((1, "two"));
    }

    [Fact]
    public void Combine_Should_Return_First_Error_When_First_Is_Failure()
    {
        var error = new DummyError();
        var r1 = Result<int>.Failure(error);
        var r2 = Result<string>.Success("ok");

        var combined = r1.Combine(r2);

        combined.IsFailure.ShouldBeTrue();
        combined.Error.ShouldBe(error);
    }

    // NEW TEST: The heart of DDD - Ensure that it aggregates notifications!
    [Fact]
    public void Combine_Should_Aggregate_Notifications_When_Both_Fail()
    {
        var notif1 = new Notification("01", "Error 1");
        var notif2 = new Notification("02", "Error 2");

        var r1 = Result<int>.Failure([notif1]);
        var r2 = Result<string>.Failure([notif2]);

        var combined = r1.Combine(r2);

        combined.IsFailure.ShouldBeTrue();
        combined.Notifications.Count.ShouldBe(2);
        combined.Notifications.ShouldContain(notif1);
        combined.Notifications.ShouldContain(notif2);
    }

    [Fact]
    public void Tap_Should_Invoke_Action_When_Success()
    {
        var value = 0;
        var result = Result<int>.Success(5);

        var tapped = result.Tap(x => value = x);

        tapped.IsSuccess.ShouldBeTrue();
        value.ShouldBe(5);
    }

    [Fact]
    public void Map_Should_Transform_Value_When_Success()
    {
        var result = Result<int>.Success(2);

        var mapped = result.Map(x => x * 10);

        mapped.IsSuccess.ShouldBeTrue();
        mapped.Value.ShouldBe(20);
    }

    // NEW TEST: Ensure that Map does not lose notifications along the way
    [Fact]
    public void Map_Should_Propagate_Notifications_When_Failure()
    {
        var notification = new Notification("01", "Bad state");
        var result = Result<int>.Failure([notification]);

        var mapped = result.Map(x => x.ToString());

        mapped.IsFailure.ShouldBeTrue();
        mapped.Notifications.ShouldContain(notification);
    }

    [Fact]
    public void MapFailure_Should_Transform_Error_When_Failure()
    {
        var originalError = new DummyError();
        var result = Result<int>.Failure(originalError);

        var mapped = result.MapFailure(_ => new DummyError());

        mapped.IsFailure.ShouldBeTrue();
        mapped.Error?.GetErrorsMessage().FirstOrDefault().ShouldBe("Object not found.");
    }

    [Fact]
    public void TryCatch_Should_Catch_Exception_And_Return_Error()
    {
        var error = new DummyError();
        var result = Result<int>.Success(5);

        var tried = result.TryCatch<int, object>(_ => throw new Exception(), error);

        tried.IsFailure.ShouldBeTrue();
        tried.Error.ShouldBe(error);
    }

    [Fact]
    public void Match_Should_Invoke_Success_Function()
    {
        var result = Result<int>.Success(42);

        // We return 'err' (which is inferred as AppError)
        var value = result.Match(
            x => $"Success: {x}",
            err => $"Fail: {err.GetErrorsMessage().FirstOrDefault()}");

        value.ShouldBe("Success: 42");
    }

    [Fact]
    public void Match_Should_Invoke_Failure_Function()
    {
        var error = new DummyError();
        var result = Result<int>.Failure(error);

        // We return 'err' (which is inferred as AppError)
        var value = result.Match(
            x => $"Success: {x}",
            err => $"Fail: {err.GetErrorsMessage().FirstOrDefault()}");

        value.ShouldBe("Fail: Object not found.");
    }

    [Fact]
    public void Bind_Async_Should_Invoke_Function_When_Success()
    {
        var resultTask = Task.FromResult(Result<int>.Success(2));

        // No await, as the Bind method returns a synchronous Result
        var bound = resultTask.Bind(x => Result<string>.Success($"Value: {x}"));

        bound.IsSuccess.ShouldBeTrue();
        bound.Value.ShouldBe("Value: 2");
    }

    [Fact]
    public void Bind_Async_Should_Not_Invoke_Function_When_Failure()
    {
        var error = new DummyError();
        var resultTask = Task.FromResult(Result<int>.Failure(error));

        // No await here either
        var bound = resultTask.Bind(x => Result<string>.Success($"Value: {x}"));

        bound.IsFailure.ShouldBeTrue();
        bound.Error.ShouldBe(error);
    }

    [Fact]
    public async Task Tap_Async_With_Task_Function_Should_Invoke_When_Success()
    {
        var wasCalled = false;
        var result = Result<int>.Success(5);

        await result.Tap(async () =>
        {
            await Task.Delay(1);
            wasCalled = true;
            return 5; // <-- Return value to satisfy Task<int> signature
        });

        wasCalled.ShouldBeTrue();
    }

    [Fact]
    public async Task Tap_Async_With_Task_Function_Should_Not_Invoke_When_Failure()
    {
        var wasCalled = false;
        var error = new DummyError();
        var result = Result<int>.Failure(error);

        await result.Tap(() =>
        {
            wasCalled = true;
            return Task.FromResult(0); // <-- Restored to Task.FromResult(0) which satisfies Task<int>
        });

        wasCalled.ShouldBeFalse();
    }

    [Fact]
    public async Task Map_Async_Should_Transform_Value_When_Success()
    {
        var result = Result<int>.Success(3);

        var mapped = await result.Map(async x =>
        {
            await Task.Delay(1);
            return x * 10;
        });

        mapped.IsSuccess.ShouldBeTrue();
        mapped.Value.ShouldBe(30);
    }

    [Fact]
    public async Task Map_Async_Should_Not_Invoke_Function_When_Failure()
    {
        var error = new DummyError();
        var result = Result<int>.Failure(error);

        var wasCalled = false;

        // We call Map DIRECTLY on the Result (without Task.FromResult)
        var mapped = await result.Map(x =>
        {
            wasCalled = true;
            return Task.FromResult(x * 10);
        });

        wasCalled.ShouldBeFalse();
        mapped.IsFailure.ShouldBeTrue();
        mapped.Error.ShouldBe(error);
    }

    [Fact]
    public async Task Tap_Async_With_Task_Result_Should_Invoke_When_Success()
    {
        var wasCalled = false;
        var resultTask = Task.FromResult(Result<string>.Success("ok"));

        await resultTask.Tap(async value =>
        {
            await Task.Delay(1);
            wasCalled = value == "ok";
        });

        wasCalled.ShouldBeTrue();
    }

    [Fact]
    public async Task Tap_Async_With_Task_Result_Should_Not_Invoke_When_Failure()
    {
        var wasCalled = false;
        var error = new DummyError();
        var resultTask = Task.FromResult(Result<string>.Failure(error));

        await resultTask.Tap(async _ =>
        {
            wasCalled = true;
            await Task.Delay(1);
        });

        wasCalled.ShouldBeFalse();
    }
}