﻿using System.Net;
using E7.Result.Errors;
using FluentAssertions;

namespace E7.Result.Tests;

public class ResultTests
{
    private class TestAppError(string detail, ErrorType errorType, string errorCodeName)
        : AppError(detail, errorType, errorCodeName)
    {
        public override List<string?> GetErrorsMessage() => [Detail];

        public override HttpStatusCode GetHttpStatusCode() => HttpStatusCode.BadRequest;
    }

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

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void IsErrorType_ShouldReturnTrueForMatchingErrorType()
    {
        var error = new TestAppError("Detail", ErrorType.ValidationRule, "ErrorCode");
        var result = Result.Failure(error);

        Assert.True(result.IsErrorType(ErrorType.ValidationRule));
    }

    [Fact]
    public void IsErrorType_ShouldReturnFalseForNonMatchingErrorType()
    {
        var error = new TestAppError("Detail", ErrorType.ValidationRule, "ErrorCode");
        var result = Result.Failure(error);

        Assert.False(result.IsErrorType(ErrorType.ValidationRule));
    }

    [Fact]
    public void ToString_ShouldReturnSuccessStringForSuccessfulResult()
    {
        var result = Result.Success();

        Assert.Equal("Success", result.ToString());
    }

    [Fact]
    public void ToString_ShouldReturnFailureStringForFailedResult()
    {
        var error = new TestAppError("Detail", ErrorType.ValidationRule, "ErrorCode");
        var result = Result.Failure(error);

        Assert.Equal($"Failure: {error}", result.ToString());
    }
}