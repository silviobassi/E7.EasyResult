using E7.EasyResult.Errors;
using E7.EasyResult.Tests.Responses;

namespace E7.EasyResult.Tests.Services;

public static class ResultSimulatorService
{
    public static Result<ObjectResponse?> GetResult(bool isSuccess)
        => isSuccess
            ? Result<ObjectResponse?>.Success(new ObjectResponse("32kj42t492gfk59df0fd8f", "123456"))
            : Result<ObjectResponse?>.Failure(new ElementNotFoundError());

    public static Result<ObjectResponse?> CheckResult(int value)
    {
        return value == 0
            ? Result<ObjectResponse?>.Failure(new ElementNotFoundError())
            : Result<ObjectResponse?>.Success(new ObjectResponse("32kj42t492gfk59df0fd8f", "123456"));
    }
}