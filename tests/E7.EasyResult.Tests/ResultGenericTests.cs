using ServicoProcessamento.Communication.E7.EasyResult;
using ServicoProcessamento.Communication.E7.EasyResult.Errors;
using Shouldly;

namespace E7.EasyResult.Tests;

public class ResultGenericTests
{
    [Fact]
    public void Success_ShouldCreateSuccessfulResultWithValue()
    {
        const string value = "SuccessValue";

        var result = Result<string>.Success(value);

        result.IsSuccess.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
        result.Value.ShouldBe(value);
        result.Error.ShouldBeNull();
    }

    [Fact]
    public void Failure_ShouldCreateFailedResultWithError()
    {
        var error = new ElementNotFoundError();

        var result = Result<string>.Failure(error);

        result.IsSuccess.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
        result.Value.ShouldBeNull();
        result.Error.ShouldBe(error);
    }

    [Fact]
    public void ImplicitConversionFromValue_ShouldCreateSuccessfulResult()
    {
        const string value = "SuccessValue";

        Result<string> result = value;

        result.IsSuccess.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
        result.Value.ShouldBe(value);
        result.Error.ShouldBeNull();
    }

    [Fact]
    public void ImplicitConversionFromError_ShouldCreateFailedResult()
    {
        var error = new ElementNotFoundError();

        Result<string> result = error!;

        result.IsSuccess.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
        result.Value.ShouldBeNull();
        result.Error.ShouldBe(error);
    }

    [Fact]
    public void ToString_ShouldReturnSuccessStringWithValue()
    {
        const string value = "SuccessValue";

        var result = Result<string>.Success(value);

        result.ToString().ShouldBe($"Success: {value}");
    }

    [Fact]
    public void ToString_ShouldReturnFailureStringWithError()
    {
        var error = new ElementNotFoundError();

        var result = Result<string>.Failure(error);

        result.ToString().ShouldBe($"Failure: {error}");
    }
}