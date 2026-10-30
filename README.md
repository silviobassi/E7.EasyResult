# E7.EasyResult

A robust .NET library for implementing the **Result Pattern** and **Railway Oriented Programming (ROP)**

## 📖 Overview

The **E7.EasyResult** package provides a semantic, predictable, and elegant approach to handling the outcomes of everyday operations. By replacing costly exceptions with a result-driven control flow, explicit failure types, and notifications, it ensures better performance, easier maintainability, and consistency across all layers of an enterprise application (from domain modeling to HTTP responses in REST APIs).

Developed to adhere to global software engineering standards and clean architectures used by major technology companies. This library focuses purely on clean code and type safety.

---

## 🎯 Objective

The main goals of adopting **E7.EasyResult** are:

1. **Eliminate Exceptions for Business Flows**: Controlling flow by throwing `Exceptions` generates high costs and degrades performance. This package allows failures to be treated as values (*first-class citizens*).
2. **Interoperable Standardization**: With abstract error classes like `AppError` and `HttpError`, a standard is created that transitions independently of the layer, simplifying responses in networking and API contexts.
3. **Accumulate Multiple Validations (Notification Pattern)**: Instead of halting at the first false validation, it's possible to aggregate notifications containing full information on all violated fields for the user.
4. **Fluidity and Composition (ROP)**: Native extensions provide method chaining with `Map`, `Bind`, `Tap`, `Ensure`, and `Match`, considerably minimizing nested `if/else` blocks resulting in a pipeline strictly focused on the "Happy Path".

---

## 📦 Installation

Available soon as a base NuGet package. (Or via your manager depending on the directory).

```bash
dotnet add package E7.EasyResult
```

---

## 🚀 Core Features & Usage

Below are the detailed capabilities of the package along with their purposes.

### 1. Creating a Success Result

**Objective:** Return an indication that a process completed smoothly or yield the value of a successful process.

**Without return value (Void):**

```csharp
using E7.EasyResult;

public Result ProcessRoutine()
{
    // Complex logic and finalization...
    return Result.Success();
}
```

**With a return value (`Result<T>`):**

```csharp
public Result<Guid> CreateEntity(string name)
{
    var id = Guid.NewGuid();
    return Result<Guid>.Create(id); // Alternative to Result<Guid>.Success(id)
}
```

### 2. Creating a Failure Result

**Objective:** Gracefully interrupt the flow, declaring a failure possessing semantic context: either an isolated domain error or an error ready to interface directly with HTTP (`ElementNotFoundError`, etc).

```csharp
using E7.EasyResult.Errors;

public Result Withdraw(decimal amount, decimal availableBalance)
{
    if (amount > availableBalance)
    {
        // Interruption handled declaratively and predictably
        return Result.Failure(new InvalidFieldsError(["Insufficient balance."])); 
    }

    return Result.Success();
}
```

#### Custom Error Typing

You can build your own typifications to enrich your business domain:

```csharp
public sealed record BusinessRuleError(string Message) 
    : HttpError(Message, HttpErrorType.BusinessRule, nameof(BusinessRuleError))
{
    public override List<string> GetErrorsMessage() => [Message];
    public override HttpStatusCode GetHttpStatusCode() => HttpStatusCode.UnprocessableEntity;
}

// Usage:
return Result<Process>.Failure(new BusinessRuleError("The contract is inactive."));
```

### 3. Native Union Cases (C# 15 / .NET 11)

`Result` and `Result<T>` are native C# unions. The cases are `Success`, `Success<T>`, and
`Failure`; use pattern matching to inspect a result and factories to construct one.

```csharp
public static Result<ObjectResponse> GetResult(bool isSuccess)
{
    if (isSuccess) 
    {
        return Result<ObjectResponse>.Success(
            new ObjectResponse("32kj42t492gfk59df0fd8f", "123456"));
    }
    
    return Result<ObjectResponse>.Failure(new ElementNotFoundError());
}
```

Inspect cases explicitly:

```csharp
return result switch
{
    Success<ObjectResponse> success => Results.Ok(success.Value),
    Failure failure => Results.BadRequest(failure.Error),
    _ => throw new InvalidOperationException("Uninitialized result.")
};
```

This is a breaking API migration from the former class hierarchy. Replace implicit
returns with `Result<T>.Success(...)` or `Result<T>.Failure(...)`, replace `.Value`
with `Success<T>.Value` after pattern matching, and use `await` for asynchronous
railway operations. Native unions require .NET 11 and C# 15.

#### Public scenario matrix

| Family | Reproducible example |
|---|---|
| Creation and inspection | `Result<T>.Success`, `Result<T>.Failure`, `switch` on `Success<T>`/`Failure` |
| Custom and HTTP errors | `BusinessRuleError`, `HttpError`, `Match` |
| Notifications | `Result.Failure(notifications)` and `Result.Failure(error, notifications)` |
| Transformation | `Map` synchronous and asynchronous |
| Composition | `Bind` and `BindAsync` |
| Validation | `Ensure` with an error or notification |
| Combination | `Combine` returning `(T1, T2)` |
| Side effects | `Tap` synchronous and asynchronous |
| Failure transformation | `MapFailure` preserving notifications |
| Exception boundary | `TryCatch` |
| Case selection | `Match` and union pattern matching |
| Async operations | `await`, pending tasks, and `ConfigureAwait(false)` in library operations |
| Migration | Before/after rules below and the explicit factory examples above |

All public families are described in this README; the executable consumer under
`.specs/features/0001-union-result/proof/` covers creation, inspection, failure
propagation, notifications, custom errors, pattern matching, mapping and pending
asynchronous work.

#### Repository architecture

- `src/E7.EasyResult/` contains the public result unions and railway extensions.
- `src/E7.EasyResult/Errors/` contains the extensible `AppError`/`HttpError` hierarchy and built-in errors.
- `tests/E7.EasyResult.Tests/` contains xUnit/Shouldly behavior tests for the public contract.
- `.specs/features/0001-union-result/proof/` contains the executable cross-assembly consumer proof.
- `.github/workflows/production.yml` restores, tests, builds, tags, packages, and publishes the NuGet artifact.

### 4. Working with Accumulated Notifications (Notification Pattern)

**Objective:** Essential for input routines or interface/application layers (DTOs), where the expectation is to validate the entire payload and return everything at once without aborting assessment upon the first incorrect field.

```csharp
public Result ValidateUserBody(UserDto user)
{
    var notifications = new List<Notification>();

    if (string.IsNullOrWhiteSpace(user.Name))
        notifications.Add(new Notification("Name", "The NAME field must be filled in."));
        
    if (user.Age < 18)
        notifications.Add(new Notification("Age", "Rules mandate User must be of legal age."));

    if (notifications.Any())
        return Result.Failure(notifications); // Returns the failure containing everything

    return Result.Success();
}
```

*(Furthermore, it is entirely possible to unify both context approaches: `Result.Failure(myError, myNotifications)`).*

---

## 🛤️ Railway Oriented Programming (ROP) Extensions

ROP tools abstract complex conditional scenarios. When a process runs on the "railway", it travels exclusively down the success track. If a car derails, it is automatically switched over to the error track, dispensing with manual `ifs`.

### Complete chaining example

The extensions are designed to be used together in application services, keeping the success path readable while failures are automatically propagated with their original error and notifications.

```csharp
public async Task<IResult> CreateOrderAsync(CreateOrderRequest request)
{
    var result = await Result<CreateOrderRequest>.Success(request)
        .Ensure(input => input.Items.Count > 0,
            new Notification("Items", "The order must contain at least one item."))
        .Map(input => new CreateOrderCommand(input.CustomerId, input.Items))
        .BindAsync(command => ValidateCustomerAsync(command))
        .BindAsync(command => ReserveInventoryAsync(command))
        .BindAsync(reservation => CreateOrderFromReservation(reservation))
        .Tap(order =>
        {
            _logger.LogInformation("Order {OrderId} created.", order.Id);
            return Task.CompletedTask;
        });

    return result.Match(
        onSuccess: order => Results.Created($"/orders/{order.Id}", order),
        onFailure: (Failure failure) => HandleFailure(failure));
}

static IResult HandleFailure(Failure failure)
{
    var error = failure.Error;
    var statusCode = error is HttpError httpError
        ? (int)httpError.GetHttpStatusCode()
        : StatusCodes.Status400BadRequest;
    if (statusCode is < 400 or > 599)
    {
        statusCode = StatusCodes.Status400BadRequest;
    }

    return Results.Problem(
        detail: error?.Message ?? "Validation failed.",
        statusCode: statusCode,
        extensions: new Dictionary<string, object?>
        {
            ["notifications"] = failure.Notifications,
            ["errorCode"] = error?.ErrorCodeName,
            ["errorType"] = error?.AppErrorType.ToString()
        });
}

private async Task<Result<CreateOrderCommand>> ValidateCustomerAsync(CreateOrderCommand command)
{
    return await _customers.ExistsAsync(command.CustomerId)
        ? Result<CreateOrderCommand>.Success(command)
        : Result<CreateOrderCommand>.Failure(new ElementNotFoundError());
}

private async Task<Result<InventoryReservation>> ReserveInventoryAsync(CreateOrderCommand command)
{
    var reservation = await _inventory.TryReserveAsync(command.Items);

    return reservation is not null
        ? Result<InventoryReservation>.Success(reservation)
        : Result<InventoryReservation>.Failure(
            new InvalidFieldsError(["One or more items are unavailable."]));
}

private Result<Order> CreateOrderFromReservation(InventoryReservation reservation)
{
    return Order.Create(reservation);
}
```

In this pipeline:

- `Ensure` blocks invalid input early and returns notifications.
- `Map` converts the request into an application command without leaving the result flow.
- `BindAsync` chains operations that may fail, whether they are synchronous or asynchronous.
- `Tap` runs side effects only when the previous steps succeeded.
- `Match` converts the final `Result<T>` into the boundary response expected by the application.

### `Map`

**Objective:** Alter the value embedded within the result, if it is in a `Success` state. If the original state was a failure, the error cascades forward without invoking the designated transformation function.

```csharp
Result<User> searchResult = SearchBase(id);

// Silently transforms a UserEntity into a DTO
Result<UserDto> dtoResult = searchResult
    .Map(u => new UserDto(u.Name, u.Email));
```

### `Bind`

**Objective:** Unlike `Map`, `Bind` chains *another* routine or function that itself will return a new `Result`. This architecture allows stacking sequential risk-prone operations transparently within a single pipeline block.

```csharp
Result<Guid> newAccount = ValidateDocument(request)
    .Bind(doc => GenerateVirtualAccount(doc))
    .Bind(account => RegisterInDatabase(account));
```

### `BindAsync`

**Objective:** Compose asynchronous application flows without leaving the result pipeline. `BindAsync` accepts both synchronous and asynchronous functions, short-circuits on the first failure, and keeps the original error and notifications available to the final consumer.

```csharp
public async Task<Result<UserDto>> RegisterUserAsync(CreateUserRequest request)
{
    return await ValidateRequest(request)
        .BindAsync(valid => EnsureEmailIsAvailableAsync(valid))
        .BindAsync(valid => _users.CreateAsync(valid.Name, valid.Email))
        .BindAsync(user => Result<UserDto>.Success(new UserDto(user.Id, user.Name, user.Email)));
}

private async Task<Result<CreateUserRequest>> EnsureEmailIsAvailableAsync(CreateUserRequest request)
{
    return await _users.ExistsAsync(request.Email)
        ? Result<CreateUserRequest>.Failure(new InvalidFieldsError(["Email already registered."]))
        : Result<CreateUserRequest>.Success(request);
}
```

The same method also works when the current value is already task-wrapped:

```csharp
public Task<Result<UserDto>> GetUserAsync(Guid id)
{
    return _users.FindAsync(id)
        .BindAsync(user => Result<UserDto>.Success(new UserDto(user.Id, user.Name, user.Email)));
}
```

### `Tap` (Side Effects)

**Objective:** Silently observe or produce "hidden" background outputs (like logging, triggering message buses, measuring metrics) reliant on successful results, without disrupting or substituting their current values.

```csharp
Result<Process> result = ClosePayroll(reference)
    .Tap(proc => _logger.LogInformation($"Payroll {proc.Id} fulfilled"));
```

### `Ensure`

**Objective:** Ensure that an assumption is upheld within a pipeline that up until this stage was running reliably. The method enforces an attached predicate condition and immediately deviates the result down the error track if the validation fails.

```csharp
Result<User> validation = ValidateSession()
    .Ensure(s => s.EndDate > DateTime.UtcNow, new BusinessRuleError("Session expired."));
```

### `Match`

**Objective:** Unpack and recover final values or delegate the complete `Failure` case to a boundary adapter. The
`Failure` overload exposes both the primary error and notifications; the legacy `AppError?` overload remains available
for gradual migration.

```csharp
public IResult FetchWithMatch(int id)
{
    return SearchUser(id)
        .Match(
            onSuccess: user => Results.Ok(user),
            onFailure: (Failure failure) => HandleFailure(failure));
}
```

Use the complete failure overload for centralized HTTP handling:

```csharp
static IResult HandleFailure(Failure failure)
{
    var error = failure.Error;
    var statusCode = error is HttpError httpError
        ? (int)httpError.GetHttpStatusCode()
        : StatusCodes.Status400BadRequest;

    return Results.Problem(
        detail: error?.Message ?? "Validation failed.",
        statusCode: statusCode,
        extensions: new Dictionary<string, object?>
        {
            ["notifications"] = failure.Notifications,
            ["errorCode"] = error?.ErrorCodeName,
            ["errorType"] = error?.AppErrorType.ToString()
        });
}
```

The handler should normalize an invalid or unexpected HTTP status to `400 Bad Request`; it must never turn a failure
into a successful response. It should also include the error code and classification when the response format supports
those fields.

Existing consumers can keep the legacy overload:

```csharp
var message = result.Match(
    onSuccess: value => value.ToString(),
    onFailure: (AppError? error) => error?.Message ?? "Validation failed.");
```

---

## 🌐 Pre-Built Extensive Error Handling

Adding upon the robust base of `AppError`, this package natively provides ready-made derivatives tailored to the .NET Web ecosystem, saving immense boilerplate domain coding hours:

- `InvalidFieldsError` (Catches requests mapping to 400 Bad Request)
- `ElementNotFoundError` (Fluent association mimicking 404 Not Found)
- `FailToCreateObjectError` (Systems warning handling 500 Internal Server error cases) and numerous other derivations mapped to `HttpErrorType`.

---

## 🏢 Identity & Support

**Enfatiza7 Consultoria em Tecnologia LTDA** is fiercely devoted to launching globally viable, enterprise-grade architectures aimed at ensuring clean, highly scalable codebase production.

🌐 **Website:** [enfatiza7.com](https://enfatiza7.com)  
✉️ **Professional Contact & Support:** [contato@enfatiza7.com](mailto:contato@enfatiza7.com)  

> *Designed and written by **Silvio Luiz Bassi** (Software Engineer and Architect), engineered to impact solutions.*  
