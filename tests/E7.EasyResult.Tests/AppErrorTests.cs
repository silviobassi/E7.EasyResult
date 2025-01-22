using System.Net;
using E7.EasyResult.Errors;
using FluentAssertions;

namespace E7.EasyResult.Tests;

public class AppErrorTests
{
    [Fact]
    public void GetErrorsMessage_ShouldReturnErrorMessage()
    {
        var error = new ElementNotFoundError();

        var result = error.GetErrorsMessage();

        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.Should().Contain("Object not found.");
        error.GetHttpStatusCode().Should().Be(HttpStatusCode.NotFound);
        error.ErrorType.Should().Be(ErrorType.NotFoundRule);
        error.ErrorCodeName.Should().Be(nameof(ElementNotFoundError));
    }

    [Fact]
    public void GetErrorsMessage_ShouldReturnListOfErrorMessages()
    {
        var error = new InvalidFieldsError(["Name required", "Email required", "Password required"]);

        error.Should().NotBeNull();
        error.GetErrorsMessage().Should().HaveCount(3);
        error.GetErrorsMessage().Should().Contain("Name required");
        error.GetErrorsMessage().Should().Contain("Email required");
        error.GetErrorsMessage().Should().Contain("Password required");
        error.GetHttpStatusCode().Should().Be(HttpStatusCode.BadRequest);
        error.ErrorType.Should().Be(ErrorType.ValidationRule);
        error.ErrorCodeName.Should().Be(nameof(InvalidFieldsError));
    }
}