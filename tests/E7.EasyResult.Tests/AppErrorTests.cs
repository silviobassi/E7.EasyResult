using System.Net;
using E7.EasyResult.Errors;
using Shouldly;

namespace E7.EasyResult.Tests;

public class AppErrorTests
{
    [Fact]
    public void GetErrorsMessage_ShouldReturnErrorMessage()
    {
        var error = new ElementNotFoundError();

        var result = error.GetErrorsMessage();

        result.ShouldNotBeNull();
        result.Count.ShouldBe(1);
        result.ShouldContain("Object not found.");
        error.GetHttpStatusCode().ShouldBe(HttpStatusCode.NotFound);
        error.ErrorType.ShouldBe(ErrorType.NotFoundRule);
        error.ErrorCodeName.ShouldBe(nameof(ElementNotFoundError));
    }

    [Fact]
    public void GetErrorsMessage_ShouldReturnListOfErrorMessages()
    {
        var error = new InvalidFieldsError(["Name required", "Email required", "Password required"]);

        error.ShouldNotBeNull();
        var messages = error.GetErrorsMessage();
        messages.Count.ShouldBe(3);
        messages.ShouldContain("Name required");
        messages.ShouldContain("Email required");
        messages.ShouldContain("Password required");
        error.GetHttpStatusCode().ShouldBe(HttpStatusCode.BadRequest);
        error.ErrorType.ShouldBe(ErrorType.ValidationRule);
        error.ErrorCodeName.ShouldBe(nameof(InvalidFieldsError));
    }
}