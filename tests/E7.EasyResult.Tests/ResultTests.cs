using FluentAssertions;
using ServicoProcessamento.Communication.E7.EasyResult;
using ServicoProcessamento.Communication.E7.EasyResult.Errors;
using static E7.EasyResult.Tests.Services.ResultSimulatorService;

namespace E7.EasyResult.Tests;

public class ResultTests
{
    [Fact]
    public void Success_ShouldCreateSuccessfulResult()
    {
        var result = Result.Success();

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().BeNull();
    }

    [Fact]
    public void Failure_ShouldCreateFailedResult()
    {
        var error = new ElementNotFoundError();
        var result = Result.Failure(error);

        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void ImplicitConversionFromError_ShouldCreateFailedResult()
    {
        var error = new ElementNotFoundError();
        Result result = error;

        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void IsErrorType_ShouldReturnTrueForMatchingErrorType()
    {
        var error = new ElementNotFoundError();
        var result = Result.Failure(error);

        result.IsErrorType(ErrorType.NotFoundRule).Should().BeTrue();
    }

    [Fact]
    public void IsErrorType_ShouldReturnFalseForNonMatchingErrorType()
    {
        var error = new ElementNotFoundError();
        var result = Result.Failure(error);

        result.IsErrorType(ErrorType.BusinessRule).Should().BeFalse();
    }

    [Fact]
    public void ToString_ShouldReturnSuccessStringForSuccessfulResult()
    {
        var result = Result.Success();
        result.ToString().Should().Be("Success");
    }

    [Fact]
    public void ToString_ShouldReturnFailureStringForFailedResult()
    {
        var error = new ElementNotFoundError();
        var result = Result.Failure(error);

        result.ToString().Should().Be($"Failure: {error}");
    }

    [Fact]
    public void Bind_ShouldReturnFailedResult()
    {
        var error = new ElementNotFoundError();
        var initialResult = Result<int>.Failure(error);

        var result = initialResult.Bind(_ => Result<int>.Success(42));

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(error);
        result.Value.Should().NotBe(42);
    }

    [Fact]
    public void Bind_SuccessResult_ReturnsSuccessResult()
    {
        var initialResult = Result<int>.Success(42);
        var result = initialResult.Bind(_ => Result<string>.Success("42"));

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("42");
        result.Error.Should().BeNull();
    }

    [Fact]
    public void MapFailure_ShouldReturn_SuccessWithResult()
    {
        var result = Result<int>.Success(1);

        var mappedResult = result.MapFailure(_ => new ElementNotFoundError());

        mappedResult.IsSuccess.Should().BeTrue();
        mappedResult.Value.Should().Be(1);
        mappedResult.Error.Should().BeNull();
    }

    [Fact]
    public void MapFailure_ShouldReturn_FailureWithResult()
    {
        var error = new ElementNotFoundError();
        var result = Result<int>.Failure(error);

        var mappedResult = result.MapFailure(_ => new ElementNotFoundError());

        mappedResult.IsSuccess.Should().BeFalse();
        mappedResult.Value.Should().Be(0);
        mappedResult.Error.Should().BeOfType<ElementNotFoundError>();
    }

    [Fact]
    public void Map_ShouldReturn_SuccessWithResult()
    {
        var result = Result<int>.Success(1);

        var mappedResult = result.Map(x => x.ToString());

        mappedResult.IsSuccess.Should().BeTrue();
        mappedResult.Value.Should().Be("1");
        mappedResult.Error.Should().BeNull();
    }

    [Fact]
    public void Map_ShouldReturn_FailureWithResult()
    {
        var error = new ElementNotFoundError();
        var result = Result<int>.Failure(error);

        var mappedResult = result.Map(x => x.ToString());

        mappedResult.IsSuccess.Should().BeFalse();
        mappedResult.Value.Should().BeNull();
        mappedResult.Error.Should().BeOfType<ElementNotFoundError>();
    }


    [Fact]
    public void Tap_ShouldReturn_SuccessWithResult()
    {
        var result = Result<int>.Success(1);
        var value = 2;

        var mappedResult = result.Tap(x => value = x);

        mappedResult.IsSuccess.Should().BeTrue();
        mappedResult.Value.Should().Be(1);
        value.Should().Be(result.Value);
        mappedResult.Error.Should().BeNull();
    }

    [Fact]
    public void Bind_ShouldReturnFailedResult_On_EnRailway()
    {
        //var result = Result<ObjectResponse>.Success(new ObjectResponse("ddd", "ccc"));
        
        var result = GetResult(false);

        string value = "42";

        result
            .Map(x => x)
            .Bind(_ => result.MapFailure(x => x))
            .Bind(_ => CheckResult(0))
            .Tap(x => value = x.Code);


        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeOfType<ElementNotFoundError>();
        result.Value.Should().NotBeNull();

    }
}