using E7.Result.Errors;
using FluentAssertions;

namespace E7.Result.Tests;

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
        var error = new TestAppError("Detail", ErrorType.ValidationRule, "ErrorCode");
        var result = Result.Failure(error);

        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void ImplicitConversionFromError_ShouldCreateFailedResult()
    {
        var error = new TestAppError("Detail", ErrorType.ValidationRule, "ErrorCode");
        Result result = error;

        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void IsErrorType_ShouldReturnTrueForMatchingErrorType()
    {
        var error = new TestAppError("Detail", ErrorType.ValidationRule, "ErrorCode");
        var result = Result.Failure(error);
        
       result.IsErrorType(ErrorType.ValidationRule).Should().BeTrue();
    }

    [Fact]
    public void IsErrorType_ShouldReturnFalseForNonMatchingErrorType()
    {
        var error = new TestAppError("Detail", ErrorType.ValidationRule, "ErrorCode");
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
        var error = new TestAppError("Detail", ErrorType.ValidationRule, "ErrorCode");
        var result = Result.Failure(error);

        result.ToString().Should().Be($"Failure: {error}");
    }
}