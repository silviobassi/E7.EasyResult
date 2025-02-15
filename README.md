# E7.EasyResult

This package provides an implementation for handling results using the Result pattern, facilitating the management of
success and failure in operations.

## Installation

To install the E7.EasyResult package, follow the steps below:

### Using .NET CLI

```bash
dotnet add package E7.EasyResult
```

### Using NuGet Package Manager Console

```bash
Install-Package E7.EasyResult
```

## Documentation

### Main Classes

### `AppError`

Abstract base class for application errors. Contains information about the error type, error code, and additional
details.

### `InvalidFieldsError`

Specific error for when the provided fields are invalid.

### `ElementNotFoundError`

Specific error for when an object or resource is not found.

### Example 2: Custom own error class

```csharp
using System.Net;

namespace E7.EasyResult.Errors;

public class CustomNotFoundError : AppError("Object not found.", ErrorType.NotFoundRule, nameof(CustomNotFoundError))
{
    public override List<string?> GetErrorsMessage() => [Detail];

    public override HttpStatusCode GetHttpStatusCode() => HttpStatusCode.NotFound;
}
```

### `Result`

Represents the result of an operation, indicating success or failure.

### `Result<T>`

Generic extension of `Result` that can contain a value of type `T` in cases of success.

### `ResultHandler`

Static class for handling matching logic between success and failure results.

### `ErrorType`

Enumeration defining the types of errors that can occur in the application:

- `BadRequestRule`: Error related to invalid requests.
- `BusinessRule`: Error related to business rules.
- `ConflictRule`: Error related to database conflicts.
- `ForbiddenAccessRule`: Error related to forbidden access.
- `NotFoundRule`: Error related to resources not found.
- `ValidationRule`: Error related to data validation.
- `UnauthorizedAccess`: Error related to unauthorized access.

## Usage Examples

### Simulation Service for Result Simulation

```csharp

public static class ResultSimulatorService
{
    public static Result<ObjectResponse?> GetResult(bool isSuccess){
        
        if(isSuccess) return new ObjectResponse("32kj42t492gfk59df0fd8f", "123456");
        
        return new CustomNotFoundError();
    }
}

public record ObjectResponse(string Id, string Codigo);
```

### Returning a Response Object

#### Getting the Result with Match

```csharp

// Get success simulation
var response = ResultSimulatorService.GetResult(true);

// Match the result
var result = response.Match(
    onSuccess: () => result,
    onFailure: (error) => error
);

Console.WriteLine($"ObjectResponse: {result.Value}")
```

#### Get the Result with Ifs

```csharp
// Check if the result is a success
if(response.IsSuccess) {
    Console.WriteLine($"Response: {result.Value}")
}
else {
    Console.WriteLine($"ObjectResponseError: {result.Error}");
    Console.WriteLine($"Message: {result.Error.GetErrorsMessage()}");
    Console.WriteLine($"StatusCode: {result.Error.GetHttpStatusCode()}");
}
```

#### Expected Output expected result for the two approaches above.

```
Response : { Id = "32kj42t492gfk59df0fd8f", Codigo = "123456" }
```

### Returning a Error Object

```csharp

// Get failure simulation
var response = ResultSimulatorService.GetResult(false);

var result = response.Match(
    onSuccess: () => result,
    onFailure: (error) => error
);

Console.WriteLine($"ObjectResponseError: {result.Error}");
Console.WriteLine($"Message: {result.Error.GetErrorsMessage()}");
Console.WriteLine($"StatusCode: {result.Error.GetHttpStatusCode()}");
```

#### Get the Result with Ifs

```csharp
// Check if the result is a failure
if(response.IsFailure) {
    Console.WriteLine($"ObjectResponseError: {result.Error}");
    Console.WriteLine($"Message: {result.Error.GetErrorsMessage()}");
    Console.WriteLine($"StatusCode: {result.Error.GetHttpStatusCode()}");
}
else {
    Console.WriteLine($"Response: {result.Error}")
}
```

#### Expected Output expected result for the two approaches above.

```     
ObjectResponseError : { ErrorType = 500, ErrorCodeName = "ElementNotFoundError" }
Message : [ "Element not found." ]
StatusCode : 404
```

### Getting the Result with Match Specific Types

#### Example 1

```csharp
var result = response.Match<IObjectResult>(
    onSuccess: () => new ObjectResult(result.Value),
    onFailure: (error) => new ObjectResultError(error)
);
```

#### Example 2

```csharp
var result = response.Match<string>(
    onSuccess: () => result.Value.ToString(),
    onFailure: (error) => error.GetErrorsMessage().FirstOrDefault()
);
```

### Creating a Result Object with Success

```csharp
var value = new ObjectResponse("32kj42t492gfk59df0fd", "123456");

var result = Result<ObjectResponse>.Success(value);

Console.WriteLine($"Response: {result.Value}")
```

#### Expected Output

```
Response : { Id = "32kj42t492gfk59df0fd8f", Codigo = "123456" }
```

### Creating a Result Object with Error

```csharp
var error = new CustomNotFoundError();

var result = Result<CustomNotFoundError>.Failure(value);

Console.WriteLine($"ObjectResponseError: {result.Error}");
```

#### Expected Output

```
ObjectResponseError : { ErrorType = 500, ErrorCodeName = "CustomNotFoundError" }
Message : [ "Element not found." ]
StatusCode : 404
```


