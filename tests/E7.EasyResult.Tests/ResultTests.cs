using E7.EasyResult.Errors;
using Shouldly;

namespace E7.EasyResult.Tests;

public sealed class ResultTests
{
    [Fact]
    public void Union_Should_Expose_Explicit_Success_Case()
    {
        var result = Result<string>.Success("value");

        result.ShouldBeOfType<Result<string>>();
        result.IsSuccess.ShouldBeTrue();
        result.Match(value => value, _ => string.Empty).ShouldBe("value");
    }

    [Fact]
    public void Union_Should_Distinguish_Null_Success_From_Default()
    {
        var success = Result<string?>.Success(null);
        var uninitialized = default(Result<string?>);

        success.IsSuccess.ShouldBeTrue();
        uninitialized.IsSuccess.ShouldBeFalse();
        Should.Throw<InvalidOperationException>(() => uninitialized.Error);
    }

    [Fact]
    public void Failure_Should_Copy_Notifications_And_Preserve_Order()
    {
        List<Notification> notifications =
        [
            new("first", "First"),
            new("second", "Second")
        ];

        var result = Result.Failure(new TestError(), notifications);
        notifications.Add(new("third", "Third"));

        result.Notifications.Select(notification => notification.Code)
            .ShouldBe(["first", "second"]);
    }

    [Fact]
    public void Failure_Constructor_Should_Copy_Notifications()
    {
        List<Notification> notifications = [new("first", "First")];
        var failure = new Failure(null, notifications);

        notifications.Add(new("second", "Second"));

        failure.Notifications.ShouldBe([new Notification("first", "First")]);
    }

    [Fact]
    public void Failure_Should_Preserve_Error_And_Notifications()
    {
        var error = new TestError();
        var notification = new Notification("field", "Invalid");
        var result = Result<string>.Failure(error, [notification]);

        result.Error.ShouldBe(error);
        result.Notifications.ShouldBe([notification]);
    }

    private sealed record TestError()
        : AppError("Test error", HttpErrorType.BadRequest, nameof(TestError))
    {
        public override List<string> GetErrorsMessage() => [Message];
    }
}
