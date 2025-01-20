using E7.Result.Errors;
using E7.Result.Tests.Responses;
using FluentAssertions;
using static E7.Result.Tests.Services.ResultSimulatorService;

namespace E7.Result.Tests;

public class ResultHandlerTests
{
    [Fact]
    public void Match_ShouldReturnOnSuccess_WhenResultIsSuccess()
    {
        var result = GetResult(true);

        var actionResult = result.Match(
            () => result,
            appError => appError
        );

        actionResult.Value.Should().Be(result.Value);
        actionResult.Error.Should().BeNull();
    }

    [Fact]
    public void Match_ShouldReturnOnFailure_WhenResultIsFailure()
    {
        var result = GetResult(false);

        var actionResult = result.Match(
            () => result,
            appError => appError
        );

        actionResult.Error.Should().Be(result.Error);
        actionResult.Value.Should().BeNull();
    }

    [Fact]
    public void Match_ShouldReturnOnSuccess_WhenResultIsSuccess_Generic()
    {
        var result = GetResult(true);

        var actionResult = result.Match<IObjectResult>(
            () => new Ok(result.Value),
            appError => new BadRequest(appError)
        );

        var okResult = (Ok)actionResult;

        actionResult.Should().BeOfType<Ok>();
        okResult.Value.Should().Be(result.Value);
    }

    [Fact]
    public void Match_ShouldReturnOnSuccess_WhenResultIsFailure_Generic()
    {
        var result = GetResult(false);

        var actionResult = result.Match<IObjectResult>(
            () => new Ok(result.Value),
            appError => new BadRequest(appError)
        );

        var badRequest = (BadRequest)actionResult;

        actionResult.Should().BeOfType<BadRequest>();
        badRequest.Error.Should().Be(result.Error);
    }
}

internal interface IObjectResult;

internal record BadRequest(AppError Error) : IObjectResult;

internal record Ok(ObjectResponse? Value) : IObjectResult;