using System.Runtime.InteropServices.JavaScript;
using E7.Result.Errors;
using FluentAssertions;

namespace E7.Result.Tests;

public class ResultHandlerTests
{
    [Fact]
    public void Match_ShouldReturnOnSuccess_WhenResultIsSuccess()
    {
        var result = Result.Success();

        var actionResult = result.Match<object>(
            () => result,
            _ => result.Error!
        );

        actionResult.Should().Be(result);
        actionResult.Should().NotBe(result.Error);
    }


    [Fact]
    public void Match_ShouldReturnOnFailure_WhenResultIsFailure()
    {
        var error = new TestAppError("Error", ErrorType.ValidationRule, nameof(TestAppError));
        var result = Result.Failure(error);

        var actionResult = result.Match<object>(
            () => result,
            _ => result.Error!
        );

        actionResult.Should().Be(result.Error);
        actionResult.Should().NotBe(result);
    }
}