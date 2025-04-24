using System.Net;
using E7.EasyResult.Errors;
using Shouldly;

namespace E7.EasyResult.Tests;

public class RailwayExtensionsTests
{
    private class DummyError()
        : AppError("Object not found.", HttpErrorType.NotFoundRule, nameof(DummyError))
    {
        public override List<string?> GetErrorsMessage()
        {
            return [Message];
        }
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

    [Fact]
    public async Task Ensure_Async_Should_Return_Same_Result_When_Predicate_Is_True()
    {
        var result = Task.FromResult(Result<string>.Success("ok"));
        var ensured = await Task.FromResult(result.Ensure(x => x == "ok", new DummyError()));

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

    [Fact]
    public void MapFailure_Should_Transform_Error_When_Failure()
    {
        var originalError = new DummyError();
        var result = Result<int>.Failure(originalError);

        var mapped = result.MapFailure(err => new DummyError());

        mapped.IsFailure.ShouldBeTrue();
        mapped.Error?.GetErrorsMessage().FirstOrDefault().ShouldBe("Object not found.");
    }

    [Fact]
    public void TryCatch_Should_Catch_Exception_And_Return_Error()
    {
        var error = new DummyError();
        var result = Result<int>.Success(5);

        // object is used to simulate an exception type
        var tried = result.TryCatch<int, object>(_ => throw new Exception(), error);

        tried.IsFailure.ShouldBeTrue();
        tried.Error.ShouldBe(error);
    }

    [Fact]
    public void Match_Should_Invoke_Success_Function()
    {
        var result = Result<int>.Success(42);

        var value = result.Match(
            x => $"Success: {x}", err => $"Fail: {err.GetErrorsMessage()}");

        value.ShouldBe("Success: 42");
    }

    [Fact]
    public void Match_Should_Invoke_Failure_Function()
    {
        var error = new DummyError();
        var result = Result<int>.Failure(error);

        var value = result.Match(
            x => $"Success: {x}", err => $"Fail: {err.GetErrorsMessage().FirstOrDefault()}");

        value.ShouldBe("Fail: Object not found.");
    }

    [Fact]
    public async Task Bind_Async_Should_Invoke_Function_When_Success()
    {
        var result = Task.FromResult(Result<int>.Success(2));

        var bound = await Task.FromResult(result.Bind(x => Result<string>.Success($"Value: {x}")));

        bound.IsSuccess.ShouldBeTrue();
        bound.Value.ShouldBe("Value: 2");
    }

    [Fact]
    public async Task Bind_Async_Should_Not_Invoke_Function_When_Failure()
    {
        var error = new DummyError();
        var result = Task.FromResult(Result<int>.Failure(error));

        var bound = await Task.FromResult(result.Bind(x => Result<string>.Success($"Value: {x}")));

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
            return 5;
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
            return Task.FromResult(0);
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

        var mapped = await result.Map(x =>
        {
            wasCalled = true;
            return Task.FromResult(x * 10);
        });

        wasCalled.ShouldBeFalse(); // Garante que a função não foi chamada
        mapped.IsFailure.ShouldBeTrue();
        mapped.Error.ShouldBe(error);
    }


    [Fact]
    public async Task Tap_Async_With_Task_Result_Should_Invoke_When_Success()
    {
        var wasCalled = false;
        var result = Task.FromResult(Result<string>.Success("ok"));

        await result.Tap(async value =>
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
        var result = Task.FromResult(Result<string>.Failure(error));

        await result.Tap(async _ =>
        {
            wasCalled = true;
            await Task.Delay(1);
        });

        wasCalled.ShouldBeFalse();
    }
}