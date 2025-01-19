

using E7.Result.Errors;
using FluentAssertions;

namespace E7.Result.Tests;

public class ResultGenericTests
{
    [Fact]
    public void Success_ShouldCreateSuccessfulResultWithValue()
    {
        const string value = "SuccessValue";
        var result = Result<string>.Success(value);

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Value.Should().Be(value);
        result.Error.Should().BeNull();
    }

    [Fact]
    public void Failure_ShouldCreateFailedResultWithError()
    {
        var error = new TestAppError("Detail", ErrorType.ValidationRule, "ErrorCode");
        var result = Result<string>.Failure(error);

        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Value.Should().BeNull();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void ImplicitConversionFromValue_ShouldCreateSuccessfulResult()
    {
        const string value = "SuccessValue";
        Result<string> result = value;

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Value.Should().Be(value);
        result.Error.Should().BeNull();
    }

    [Fact]
    public void ImplicitConversionFromError_ShouldCreateFailedResult()
    {
        var error = new TestAppError("Detail", ErrorType.ValidationRule, "ErrorCode");
        Result<string> result = error!;

        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Value.Should().BeNull();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void ToString_ShouldReturnSuccessStringWithValue()
    {
        const string value = "SuccessValue";
        var result = Result<string>.Success(value);

        result.ToString().Should().Be($"Success: {value}");
    }

    [Fact]
    public void ToString_ShouldReturnFailureStringWithError()
    {
        var error = new TestAppError("Detail", ErrorType.ValidationRule, "ErrorCode");
        var result = Result<string>.Failure(error);

        result.ToString().Should().Be($"Failure: {error}");
    }
}