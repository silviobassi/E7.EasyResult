using E7.Result.Errors;
using E7.Result.Tests.Responses;

namespace E7.Result.Tests.Services;

public static class ResultSimulatorService
{
    public static Result<ObjectResponse?> GetResult(bool isSuccess)
        => isSuccess ? new ObjectResponse("32kj42t492gfk59df0fd8f", "123456") : new NotFoundError();
}