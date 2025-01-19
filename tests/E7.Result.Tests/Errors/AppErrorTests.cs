using System.Net;
using E7.Result.Errors;
using FluentAssertions;

namespace E7.Result.Tests.Errors;

public class AppErrorTests
{
    [Fact]
    public void GetErrorsMessage_ShouldReturnListOfErrorMessages()
    {
        var error = new TestAppError("Detail", ErrorType.ValidationRule, "ErrorCode");

        var result = error.GetErrorsMessage();

        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.Should().Contain("Detail");
    }

    [Fact]
    public void GetHttpStatusCode_ShouldReturnBadRequest()
    {
        var error = new TestAppError("Detail", ErrorType.BadRequestRule, nameof(TestAppError));

        var resultError = error.GetHttpStatusCode();
        var resultErrorType = error.ErrorType;

        resultError.Should().Be(HttpStatusCode.BadRequest);
        resultErrorType.Should().Be(ErrorType.BadRequestRule);
    }
}