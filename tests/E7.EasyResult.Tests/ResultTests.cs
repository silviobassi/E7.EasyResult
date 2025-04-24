using E7.EasyResult.Errors;
using Shouldly;

namespace E7.EasyResult.Tests;

public class ResultTests
{
    [Fact]
    public void Success_ShouldCreateSuccessfulResult()
    {
        var result = Result.Success();

        result.IsSuccess.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
        result.Error.ShouldBeNull();
    }

    [Fact]
    public void Failure_ShouldCreateFailedResult()
    {
        var error = new ElementNotFoundError();
        var result = Result.Failure(error);

        result.IsSuccess.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(error);
    }

    [Fact]
    public void ImplicitConversionFromError_ShouldCreateFailedResult()
    {
        var error = new ElementNotFoundError();
        Result result = error;

        result.IsSuccess.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(error);
    }

    [Fact]
    public void IsErrorType_ShouldReturnTrueForMatchingErrorType()
    {
        var error = new ElementNotFoundError();
        var result = Result.Failure(error);

        result.IsErrorType(HttpErrorType.NotFoundRule).ShouldBeTrue();
    }

    [Fact]
    public void IsErrorType_ShouldReturnFalseForNonMatchingErrorType()
    {
        var error = new ElementNotFoundError();
        var result = Result.Failure(error);

        result.IsErrorType(HttpErrorType.BusinessRule).ShouldBeFalse();
    }

    [Fact]
    public void ToString_ShouldReturnSuccessStringForSuccessfulResult()
    {
        var result = Result.Success();
        result.ToString().ShouldBe("Success");
    }

    [Fact]
    public void ToString_ShouldReturnFailureStringForFailedResult()
    {
        var error = new ElementNotFoundError();
        var result = Result.Failure(error);

        result.ToString().ShouldBe($"Failure: {error}");
    }

    [Fact]
    public void Bind_ShouldReturnFailedResult()
    {
        var error = new ElementNotFoundError();
        var initialResult = Result<int>.Failure(error);

        var result = initialResult.Bind(_ => Result<int>.Success(42));

        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldBe(error);
        result.Value.ShouldNotBe(42);
    }

    [Fact]
    public void Bind_SuccessResult_ReturnsSuccessResult()
    {
        var initialResult = Result<int>.Success(42);
        var result = initialResult.Bind(_ => Result<string>.Success("42"));

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe("42");
        result.Error.ShouldBeNull();
    }

    [Fact]
    public void MapFailure_ShouldReturn_SuccessWithResult()
    {
        var result = Result<int>.Success(1);

        var mappedResult = result.MapFailure(_ => new ElementNotFoundError());

        mappedResult.IsSuccess.ShouldBeTrue();
        mappedResult.Value.ShouldBe(1);
        mappedResult.Error.ShouldBeNull();
    }

    [Fact]
    public void MapFailure_ShouldReturn_FailureWithResult()
    {
        var error = new ElementNotFoundError();
        var result = Result<int>.Failure(error);

        var mappedResult = result.MapFailure(_ => new ElementNotFoundError());

        mappedResult.IsSuccess.ShouldBeFalse();
        mappedResult.Value.ShouldBe(0);
        mappedResult.Error.ShouldBeOfType<ElementNotFoundError>();
    }

    [Fact]
    public void Map_ShouldReturn_SuccessWithResult()
    {
        var result = Result<int>.Success(1);

        var mappedResult = result.Map(x => x.ToString());

        mappedResult.IsSuccess.ShouldBeTrue();
        mappedResult.Value.ShouldBe("1");
        mappedResult.Error.ShouldBeNull();
    }

    [Fact]
    public void Map_ShouldReturn_FailureWithResult()
    {
        var error = new ElementNotFoundError();
        var result = Result<int>.Failure(error);

        var mappedResult = result.Map(x => x.ToString());

        mappedResult.IsSuccess.ShouldBeFalse();
        mappedResult.Value.ShouldBeNull();
        mappedResult.Error.ShouldBeOfType<ElementNotFoundError>();
    }

    [Fact]
    public void Tap_ShouldReturn_SuccessWithResult()
    {
        var result = Result<int>.Success(1);
        var value = 2;

        var mappedResult = result.Tap(x => value = x);

        mappedResult.IsSuccess.ShouldBeTrue();
        mappedResult.Value.ShouldBe(1);
        value.ShouldBe(result.Value);
        mappedResult.Error.ShouldBeNull();
    }
}
