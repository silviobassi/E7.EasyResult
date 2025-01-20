using E7.Result.Errors;
using E7.Result.Tests.Responses;
using FluentAssertions;

namespace E7.Result.Tests;

public class ResultSTests
{
    [Fact]
    public void Success_ShouldCreateSuccessfulResult()
    {
        var result = ResultS.Success();

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().BeNull();
    }

    [Fact]
    public void Failure_ShouldCreateFailedResult()
    {
        var error = new NotFoundError();
        var result = ResultS.Failure(error);

        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void ImplicitConversionFromError_ShouldCreateFailedResult()
    {
        var error = new NotFoundError();
        ResultS resultS = error;

        resultS.IsSuccess.Should().BeFalse();
        resultS.IsFailure.Should().BeTrue();
        resultS.Error.Should().Be(error);
    }

    [Fact]
    public void IsErrorType_ShouldReturnTrueForMatchingErrorType()
    {
        var error = new NotFoundError();
        var result = ResultS.Failure(error);

        result.IsErrorType(ErrorType.NotFoundRule).Should().BeTrue();
    }

    [Fact]
    public void IsErrorType_ShouldReturnFalseForNonMatchingErrorType()
    {
        var error = new NotFoundError();
        var result = ResultS.Failure(error);

        result.IsErrorType(ErrorType.BusinessRule).Should().BeFalse();
    }

    [Fact]
    public void ToString_ShouldReturnSuccessStringForSuccessfulResult()
    {
        var result = ResultS.Success();
        result.ToString().Should().Be("Success");
    }

    [Fact]
    public void ToString_ShouldReturnFailureStringForFailedResult()
    {
        var error = new NotFoundError();
        var result = ResultS.Failure(error);

        result.ToString().Should().Be($"Failure: {error}");
    }
}