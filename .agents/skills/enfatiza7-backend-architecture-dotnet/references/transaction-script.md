
# Enfatiza7 Transaction Script Structure

Use this skill when an Enfatiza7 backend module follows Transaction Script architecture.

Transaction Script is the anti-ceremony option. Use it for simple procedural flows where a domain model, aggregate, or active record would be artificial.

## When To Use

Use Transaction Script when:

- the flow is administrative or procedural;
- rules are simple and sequential;
- there is no rich entity lifecycle;
- there are no complex invariants;
- there is no aggregate pressure;
- there is no active record behavior;
- a `Domain/` folder would only wrap table rows or DTOs.

Common examples:

- tenant settings;
- business hours;
- feature flags;
- consultant registration;
- consultant availability administration;
- simple capacity changes;
- basic admin CRUD with validation.

## Canonical Structure

Use only three layers by default:

```txt
Transaction Script module
│
├── DependencyInjection
│   └── <ModuleName>Module
│
├── Endpoints
│   ├── <ResourceName>Endpoints
│   └── <AnotherResourceName>Endpoints
│
├── Application
│   ├── Commands
│   │   └── <UseCaseName>Command
│   │
│   ├── Queries
│   │   └── <UseCaseName>Query
│   │
│   ├── Handlers
│   │   └── <UseCaseName>Handler
│   │
│   ├── Validators
│   │   └── <UseCaseName>Validator
│   │
│   ├── Responses
│   │   └── <UseCaseName>Response
│   │
│   └── Errors
│       └── <UseCaseName>Errors
│
└── Infrastructure
    ├── Persistence
    ├── Repositories
    └── ExternalAdapters
```

Do not create `Domain/` by default.

Do not create aggregates, active records, domain services, factories, or value-object ceremony unless real domain pressure appears.

## Default Flow

Keep the flow readable top-to-bottom:

```txt
Endpoint
│
└── Application Handler
    │
    ├── validate command/query
    ├── load data
    ├── execute procedural rule
    ├── call infrastructure/external boundary if needed
    ├── persist result
    └── return Result/Response
```

Default dependency flow:

```txt
Endpoint -> Application Handler
Application Handler -> Infrastructure
```

Use direct internal references between module layers unless an external boundary must be protected.

## Endpoint Rules

Endpoints should:

- map routes;
- bind requests;
- call one handler or application service;
- translate `Result` to HTTP;
- stay thin.

Endpoints should not contain business rules, data access, or infrastructure details.

## Handler Rules

Handlers are transaction scripts:

- one handler represents one use case;
- the handler may execute procedural logic directly;
- logic should be easy to read from top to bottom;
- simple local calculations may stay in the handler;
- repeated meaningful logic may be extracted to private methods or application helpers;
- do not extract services for one-line logic.

Do not create handler interfaces by default.

## No Artificial Domain

Do not upgrade Transaction Script into Domain Model.

Avoid:

```txt
Domain/
  Entities/
  Aggregates/
  DomainServices/
  Factories/
  Specifications/
```

unless there is real domain behavior that justifies leaving Transaction Script.

Do not create entities just to wrap table rows. Use persistence models, DTOs, or simple records when behavior is not needed.

## Abstractions And Ports Rule

Do not create ports between internal layers.

Create abstractions only for transversal concerns, infrastructure dependencies, or external boundaries.

Create abstractions for:

- external providers;
- secret storage;
- email/message providers;
- event bus or external queue;
- clock/current user/current tenant shared across modules;
- repository interface only when it improves testing, DI, or boundary clarity.

For integration with another module or bounded context, use the provider module's `Public/` surface when direct dependency
is allowed, or compose both modules through the host. Do not create a consumer-owned port just to call another module.

Do not create abstractions for:

- commands;
- queries;
- handlers;
- simple validators;
- one-line calculations;
- simple CRUD;
- calls internal to the same module;
- hypothetical future variants.

Repository guidance:

```txt
Simplest acceptable style:
Application Handler -> concrete Infrastructure repository

More testable style:
Application Handler -> repository interface
Infrastructure -> repository implementation
```

Repository interfaces are allowed, not mandatory.

## Error Handling

Use the project's error contract for expected business failures (`E7.EasyResult` only when adopted):

- record not found;
- duplicate record;
- invalid administrative state;
- invalid input;
- disabled tenant;
- unavailable consultant;
- configuration conflict;
- missing permission or feature entitlement.

Use exceptions for technical failures:

- database outage;
- timeout;
- provider unavailable;
- unexpected serialization failure;
- programming bug.

Do not throw exceptions for normal business branching.

## Pattern Selection

Use this quick check before adding ceremony:

```txt
Simple procedural flow? -> Transaction Script
Record has lifecycle behavior? -> Active Record
Rules are volatile per use case? -> Vertical Slice
Rich invariants/core domain? -> Domain Model
Event history/rebuild/projections? -> CQRS/Event Sourcing
```

If a Transaction Script starts accumulating stateful lifecycle methods, consider Active Record.

If rules become volatile by use case and need feature locality, consider Vertical Slice.

If invariants become central and complex, consider Domain Model.

## Anti-Over-Engineering Rule

Do not create:

- artificial `Domain/`;
- interfaces for handlers;
- factories for simple DTOs;
- domain services for procedural CRUD;
- value objects with no invariant;
- specifications for simple filters;
- ports for internal calls;
- abstractions for imagined future variations.

Prefer the simplest readable script that validates, loads, changes, persists, and returns.

## Acceptance Criteria

A Transaction Script module is well structured when:

- it has only `Endpoints`, `Application`, and `Infrastructure` layers by default;
- no artificial `Domain/` folder exists;
- each handler/use case is readable top-to-bottom;
- endpoints are thin;
- expected failures follow the project's error contract (`E7.EasyResult` only when adopted);
- infrastructure details stay in `Infrastructure`;
- abstractions exist only for infrastructure, transversal concerns, or external boundaries;
- no DDD ceremony appears without real domain pressure.


