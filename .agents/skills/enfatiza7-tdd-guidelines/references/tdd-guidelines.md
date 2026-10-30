# TDD Guidelines

## Description

Apply Test-Driven Development (TDD) to drive the design of production code through small, incremental, and verifiable steps. Tests are the primary specification of behavior and should guide implementation, not validate it afterward.

Reference: Kent Beck, Test-Driven Development: By Example.

## Objective

Produce code that is:

- Correct.
- Testable.
- Loosely coupled.
- Focused on behavior.
- Easy to refactor.
- Free from unnecessary complexity.

## Workflow

Always follow the Red -> Green -> Refactor cycle.

### 1. Red

- Write one failing test describing the desired behavior.
- Keep the test as small as possible.
- Test one behavior at a time.

### 2. Green

- Write the simplest implementation that makes the test pass.
- Do not optimize.
- Do not generalize prematurely.

### 3. Refactor

- Improve readability.
- Remove duplication.
- Rename when necessary.
- Extract methods or classes only when they simplify the design.
- Ensure all tests remain green.

## Testing Principles

Tests should:

- Verify observable behavior.
- Be deterministic.
- Be isolated.
- Be repeatable.
- Execute quickly.
- Be easy to understand.
- Have a single reason to fail.

## Prefer

- One assertion per behavior.
- Arrange / Act / Assert structure.
- Descriptive test names.
- Independent tests.
- Domain language in test names.

## Naming

Use the project's naming convention. If none exists, a descriptive behavior-oriented name is sufficient; for example:

```text
MethodName_ShouldExpectedBehavior_WhenCondition
```

Example:

```text
CreateLead_ShouldAssignConsultant_WhenConsultantIsAvailable
```

## Test Scope

Prefer testing:

- Business rules.
- Domain behavior.
- Application use cases.
- Edge cases.
- Failure scenarios.

Avoid testing:

- Private methods.
- Framework behavior.
- ORM internals.
- Dependency Injection configuration.
- Simple property getters/setters.
- Auto-generated code.

## Mocks

Mock only true collaborators.

Never mock:

- Value objects.
- Domain entities.
- Collections.
- Simple DTOs.

Prefer real implementations whenever practical.

Avoid over-mocking.

## Design Feedback

When tests become difficult to write, treat it as feedback that production code may have:

- High coupling.
- Multiple responsibilities.
- Hidden dependencies.
- Excessive complexity.

Refactor the design instead of weakening the tests.

## Coverage

Coverage is a consequence, not a goal.

Do not write meaningless tests solely to increase coverage.

## Priority

Test in this order:

1. Business rules.
2. Domain invariants.
3. Application services.
4. Integration points.
5. Infrastructure, only where valuable.

## Refactoring

Refactor only with all tests passing.

Never change behavior during refactoring.

## Integration Tests

Use integration tests to verify:

- Database mappings.
- ORM mapping and configuration when used by the project.
- External APIs.
- Messaging.
- File system.
- Caching.

The balance of unit and integration tests follows the project's architecture and test strategy; TDD does not impose a fixed ratio.

## Never

- Do not write implementation-driven tests.
- Do not assert internal implementation details.
- Do not test private methods directly.
- Do not introduce production code solely to satisfy tests.
- Do not duplicate production logic inside tests.
- Do not depend on execution order.
- Do not create brittle tests coupled to implementation.

## AI Guidelines

When generating code:

- Start by identifying the expected behavior.
- Produce the failing test first.
- Generate only the minimum production code required.
- Refactor after all tests pass.
- Prefer readability over clever assertions.
- Explain trade-offs if multiple testing approaches exist.
