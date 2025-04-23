using E7.EasyResult.Tests.Responses;
using ServicoProcessamento.Communication.E7.EasyResult;
using ServicoProcessamento.Communication.E7.EasyResult.Errors;
using Shouldly;
using static E7.EasyResult.Tests.Services.ResultSimulatorService;

namespace E7.EasyResult.Tests;

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

        actionResult.Value.ShouldBe(result.Value);
        actionResult.Error.ShouldBeNull();
    }

    [Fact]
    public void Match_ShouldReturnOnFailure_WhenResultIsFailure()
    {
        var result = GetResult(false);

        var actionResult = result.Match(
            () => result,
            appError => appError
        );

        actionResult.Error.ShouldBe(result.Error);
        actionResult.Value.ShouldBeNull();
    }

    [Fact]
    public void Match_ShouldReturnOnSuccess_WhenResultIsSuccess_Generic()
    {
        var result = GetResult(true);

        var actionResult = result.Match<IObjectResult>(
            () => new Ok(result.Value),
            appError => new BadRequest(appError)
        );

        actionResult.ShouldBeOfType<Ok>();
        var okResult = (Ok)actionResult;
        okResult.Value.ShouldBe(result.Value);
    }

    [Fact]
    public void Match_ShouldReturnOnSuccess_WhenResultIsFailure_Generic()
    {
        var result = GetResult(false);

        var actionResult = result.Match<IObjectResult>(
            () => new Ok(result.Value),
            appError => new BadRequest(appError)
        );

        actionResult.ShouldBeOfType<BadRequest>();
        var badRequest = (BadRequest)actionResult;
        badRequest.Error.ShouldBe(result.Error);
    }
}

internal interface IObjectResult;

internal record BadRequest(AppError Error) : IObjectResult;

internal record Ok(ObjectResponse? Value) : IObjectResult;
