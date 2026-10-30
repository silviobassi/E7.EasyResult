
# Enfatiza7 C#/.NET Coding Style

Use modern C# by default, explicit syntax when clarity asks, and abstractions only when real pressure exists.

Treat Clean Code, SOLID, analyzers, and SonarQube as decision aids. Do not turn them into ceremony that makes the code harder to understand.

## Core Rule

Prefer:

```txt
clarity > consistency theater
simplicity > premature abstraction
correctness > clever syntax
measured performance > speculative optimization
domain intent > generic patterns
```

Use modern C# features when they reduce noise or improve safety.

Avoid modern syntax when it hides the type, lifecycle, invariant, or business meaning.

## Language Version And Tooling

Target the newest stable C# and .NET version approved by the project.

Enforce style with:

- `.editorconfig`;
- Roslyn analyzers;
- .NET code analysis;
- SonarQube/SonarCloud;
- IDE formatting;
- CI quality gates when available.

Do not rely on review comments for rules that tools can enforce.


## Visibility And Boundaries

Use `internal` by default for implementation classes and types.

Use `public` only when the type is intentionally transversal, part of a stable contract, or explicitly crosses a bounded-context, aggregate-root, module, API, frontend, package, or shared-kernel boundary.

When an existing architecture test protects this visibility rule, run it for relevant boundary changes. Add a new guardrail only if the project states an explicit, valuable rule that can be checked deterministically and is not already covered.

## File Shape

Use:

- file-scoped namespaces;
- `using` directives outside namespaces;
- nullable reference types enabled;
- one primary type per file unless tiny supporting types are clearly local;
- English identifiers for code, namespaces, members, and public APIs.

Prefer this:

```csharp
namespace Enfatiza7.Example;

internal sealed class ExampleService
{
}
```

## Naming

Use:

- `PascalCase` for types, methods, properties, events, enum members, and constants exposed as members;
- `camelCase` for local variables and parameters;
- `_camelCase` for private instance fields;
- `PascalCase` for private static readonly fields.

Examples:

```csharp
private readonly IClock _clock;
private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);

public string DisplayName { get; init; } = string.Empty;
```

Do not use `_propertyName` for properties.

Name things by business meaning, not technical type:

```csharp
LeadId leadId;
```

Avoid:

```csharp
string leadIdString;
List<Lead> leadList;
```

unless the type distinction is actually the point.

## `var` Versus Explicit Types

Use `var` when the type is obvious from the right side:

```csharp
var lead = new Lead();
var name = "Ada";
var count = 10;
var now = DateTimeOffset.UtcNow;
```

Use explicit types when the return type is not obvious:

```csharp
LeadSummary summary = await service.GetSummaryAsync(leadId, cancellationToken);
Result<LeadId> result = LeadId.Create(input);
```

Use explicit collection types for empty collection expressions:

```csharp
List<LeadId> leadIds = [];
Dictionary<LeadId, LeadSummary> summaries = [];
```

Avoid:

```csharp
var leadIds = [];
```

because it does not communicate the intended type.

Use `var` in LINQ projections when anonymous or deeply generic types improve readability.

## Collections

Prefer collection expressions for initialization:

```csharp
string[] codes = ["A", "B", "C"];
List<string> names = [];
HashSet<LeadId> processedLeadIds = [];
```

Prefer spread collection expressions for copying/combining collections:

```csharp
var allLeads = [..activeLeads, ..pendingLeads];
```

Prefer `[..items]` when the intent is to copy or materialize into a new collection and the target type is clear:

```csharp
List<Lead> leads = [..queryResult];
```

Use `.ToList()` when it communicates LINQ materialization more clearly, when the type target is not obvious, or when the surrounding code already uses LINQ fluently:

```csharp
var qualifiedLeads = leads
    .Where(lead => lead.IsQualified)
    .ToList();
```

Do not use collection expressions to be clever. Use the form that makes intent most obvious.

Expose collection properties as read-only where possible:

```csharp
public IReadOnlyList<Message> Messages => _messages;
```

Avoid `List<T>` in public APIs unless mutation is part of the contract.

## Object Creation And Initialization

Use target-typed `new()` when the type is clear:

```csharp
Dictionary<string, int> counters = new();
```

Use object initializers for simple data:

```csharp
var request = new CreateLeadRequest
{
    Name = name,
    PhoneNumber = phoneNumber
};
```

Use `required` and `init` for DTOs/configuration models that must be initialized:

```csharp
public sealed class LeadOptions
{
    public required string DefaultCountryCode { get; init; }
}
```

Do not use `required` to compensate for weak domain construction. Domain types should protect invariants through factories, constructors, or behavior.

## Primary Constructors

Use primary constructors for simple classes, handlers, services, and records when they reduce boilerplate:

```csharp
internal sealed class CreateLeadHandler(
    ILeadRepository repository,
    IClock clock)
{
}
```

Use primary constructors for records naturally:

```csharp
public sealed record LeadSummary(LeadId LeadId, string DisplayName);
```

Avoid primary constructors when:

- constructor logic is non-trivial;
- parameters need validation before assignment;
- many dependencies make the signature noisy;
- derived fields need careful initialization;
- the class has rich lifecycle or domain behavior;
- the syntax hides too much from the reader.

Use a traditional constructor when it tells the story better.

## Records, Classes, And Structs

Use `record` for immutable data with value equality:

- commands;
- queries;
- responses;
- read models;
- DTOs;
- simple domain events when serialization/versioning allows it.

Use `class` for behavior and identity:

- services;
- handlers;
- aggregates;
- active records;
- entities with lifecycle;
- infrastructure adapters.

Use `readonly record struct` or `readonly struct` only for small immutable value objects where value semantics matter and `default` is safe or explicitly handled.

Do not choose `struct` only for perceived performance. Prefer reference types when serialization, ORM mapping, nullability, invalid default values, identity, lifecycle, or versioning make them safer.

Value Object details belong to a separate guide. Until then:

- prefer immutability;
- prefer value equality;
- use value types only when `default` is safe or explicitly handled;
- use reference types when serialization, ORM mapping, nullability, or invalid default values make them safer.

Do not make event-store domain events `struct` by default. Prefer immutable, serializable, versionable reference types for event history.

## Control Flow

Omit braces for single-statement control-flow bodies.

Prefer:

```csharp
if (result.IsFailure)
    return result.Error!;
```

Use braces when the body has multiple statements, when an `else if`/`else` chain would become harder to scan, or when omitting braces would reduce clarity.

## Nullability And Validation

Keep nullable reference types enabled.

Avoid null-forgiving `!` unless there is a clear framework or serializer boundary reason.

Validate at boundaries:

- endpoints;
- handlers;
- factories;
- external adapters;
- persistence mapping.

Do not let invalid state enter the domain just to simplify binding or serialization.

Prefer:

```csharp
if (string.IsNullOrWhiteSpace(name))
{
    return new InvalidNameError();
}
```

over allowing invalid values and checking everywhere later.

## Async

Use `Async` suffix for asynchronous methods.

Pass `CancellationToken` through application, infrastructure, persistence, and external calls:

```csharp
Task<Result<LeadId>> HandleAsync(CreateLeadCommand command, CancellationToken cancellationToken);
```

Do not block async code:

```txt
avoid .Result
avoid .Wait()
avoid GetAwaiter().GetResult()
```

Return `Task` or `Task<T>`; never return `null` from async contracts.

Use `ValueTask` only when there is a measured reason or a framework API requires it.

Use `ConfigureAwait(false)` in reusable libraries when appropriate. Do not add it blindly to ASP.NET Core application code.

## LINQ

Use LINQ when it improves readability.

Use loops when they are clearer, cheaper, or avoid multiple enumeration.

Prefer:

```csharp
if (ids.Contains(leadId))
{
}
```

over:

```csharp
if (ids.Any(id => id == leadId))
{
}
```

Filter before sorting:

```csharp
var recentQualifiedLeads = leads
    .Where(lead => lead.IsQualified)
    .OrderByDescending(lead => lead.QualifiedAt)
    .ToList();
```

Avoid hidden repeated enumeration. Materialize once when needed.

## Strings, Dates, And Culture

Use string interpolation for short strings.

Use `StringBuilder` for repeated concatenation in loops.

Use raw string literals for JSON, SQL, templates, and multi-line text.

Use `DateTimeOffset` for instants.

Use UTC for stored timestamps.

Use `IClock` or equivalent testable time provider for domain/application logic.

Use overloads with `StringComparison`, `CultureInfo`, or `IFormatProvider` when comparing, parsing, or formatting user-visible or culture-sensitive data.

## Exceptions And Results

Use the project's declared error contract for expected business failures; `E7.EasyResult` is one option when adopted.

Use exceptions for technical failures and programming errors.

Do not catch `Exception` unless:

- adding boundary logging;
- translating at an application boundary;
- applying resilience;
- rethrowing correctly.

When logging caught exceptions, pass the exception object to the logger.

## Clean Code And SOLID

Apply Clean Code and SOLID pragmatically.

Use SRP as:

```txt
one dominant reason to change
```

not:

```txt
one tiny class per line of logic
```

Use DIP for real boundaries:

- persistence;
- external providers;
- clock/current user/current tenant;
- messaging;
- file/storage;
- cross-module boundaries.

Do not create interfaces for:

- every handler;
- every service;
- simple calculations;
- one implementation with no boundary;
- hypothetical future variants.

Prefer direct dependencies inside the same module unless there is real boundary pressure.

## Performance

Optimize when:

- a profiler points to the code;
- the path is known hot;
- allocation volume is high;
- Sonar/analyzers flag a meaningful issue;
- the simpler code is also faster.

Do not optimize by making normal code obscure.

Prefer low-cost wins:

- avoid repeated enumeration;
- avoid unnecessary `.ToList()`;
- use `Contains` over `Any` for simple membership;
- avoid allocating in loops;
- use `StringBuilder` for loop concatenation;
- choose collection types by lookup pattern;
- use async I/O instead of blocking.

## SonarQube And Analyzer Guidance

Treat Sonar issues by priority:

```txt
Security/Vulnerability
Bug
Reliability
Maintainability
Style
```

Fix security and bug findings by default.

For maintainability/style findings, fix when the change improves clarity or consistency.

Suppress only with a short justification.

Do not satisfy a rule by making the code harder to understand. Prefer local justification when domain clarity wins.

## Comments

Write comments for:

- non-obvious business decisions;
- invariants;
- tradeoffs;
- external quirks;
- security-sensitive choices;
- performance-sensitive choices.

Avoid comments that restate the code.

## Acceptance Criteria

C#/.NET code follows this style when:

- modern syntax reduces noise without hiding meaning;
- explicit types appear where they improve clarity;
- collection expressions are used where natural;
- `ToList()` is used intentionally, not habitually;
- primary constructors are used only when they keep the class readable;
- private fields use `_camelCase`;
- properties use `PascalCase`;
- async code is not blocked;
- nullability is respected;
- business failures use result types;
- technical failures use exceptions;
- abstractions exist only under real pressure;
- Sonar/analyzer findings are fixed or justified;
- performance choices are readable or measured.

