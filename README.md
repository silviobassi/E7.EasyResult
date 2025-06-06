# E7.EasyResult

This package provides an implementation for handling results using the Result pattern, facilitating the management of
success and failure in operations.

## Installation

To install the E7.EasyResult package, follow the steps below:

### With .NET CLI

```bash
dotnet add package E7.EasyResult
```

### With NuGet Package Manager Console

```bash
Install-Package E7.EasyResult
```

## Usage the Results Pattern

### `AppError`

Serves as the abstract base class for all structured application errors,
offering a consistent model for categorizing, identifying, and communicating issues
across different layers of an application (e.g., domain, application, infrastructure).

### `HttpError`

Represents an application error that is specifically intended to be mapped to an HTTP response. This class extends of
_AppError_

### `InvalidFieldsError`

Specific error for when the provided fields are invalid. This class extends of _HttpError_

### `ElementNotFoundError`

Specific error for when an object or resource is not found. This class extends of _HttpError_

### Example 2: Custom own error class

```csharp
namespace E7.EasyResult.Errors;

public abstract class DomainError(string? message, Enum appErrorType, string errorCodeName)
    : AppError(message, appErrorType, errorCodeName)
{
}

public class BusinessError() : DomainError("Required field.", DomainErrorType.ValidationError, nameof(BusinessError))
{
    public override List<string?> GetErrorsMessage() => [Message];
}
```

**_Note_** that you can create your own enum error type `DomainErrorType.ValidationError`
### `Result`

Represents the result of an operation, indicating success or failure.

### `Result<T>`

Generic extension of `Result` that can contain a value of type `T` in cases of success.

### `HttpErrorType`

Enumeration defining the types of errors that can occur in the application:

- `BadRequestRule`: Error related to invalid requests.
- `BusinessRule`: Error related to business rules.
- `ConflictRule`: Error related to database conflicts.
- `ForbiddenAccessRule`: Error related to forbidden access.
- `NotFoundRule`: Error related to resources not found.
- `ValidationRule`: Error related to data validation.
- `UnauthorizedAccess`: Error related to unauthorized access.

**Note:** You can create your own error types

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

## This package provides resources for applying Railway Pattern

Railway Oriented Programming (ROP) is a functional pattern that models execution as a two-track system: success and
failure. It builds on the Result pattern, where each function returns a `Result<T>`, avoiding exceptions. This enables
safe, fluent composition of operations with automatic error propagation. The concept was popularized by software
engineer Scott Wlaschin, and you can learn more about it at
the [pattern's link](https://fsharpforfunandprofit.com/rop/).

### Simple Usage/Example

```csharp
Result<User> userResult = GetUser(id)
    .Ensure(user => user.IsInactive, new BusinessRule("Inactive User"))
    .Map(user => user.Name.ToUpper())
    .Tap(name => Log(name));

```

To delve deeper into the pattern, take a look at the source code and documentation.

🤝 Contributing
This package is open source and welcomes contributions! Fork the repository, create a branch, and submit a pull request
with your enhancements.

## 👤 Author
**Silvio Luiz Bassi**  
_Software Engineer_  
📧 Contato: [LinkedIn](https://www.linkedin.com/in/silvio-bassi/)  
🏢 Empresa: **Enfatiza7 Consultoria em Tecnologia da Informação LTDA**

## 📄 License
This project is licensed under the MIT License. See
the [LICENSE](https://github.com/silviobassi/E7.EasyResult?tab=MIT-1-ov-file) file for details.

