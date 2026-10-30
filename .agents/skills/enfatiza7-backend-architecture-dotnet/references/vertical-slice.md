
# Enfatiza7 Vertical Slice Structure

Use this skill when an Enfatiza7 backend module follows Vertical Slice architecture.

The goal is to preserve feature locality without turning the module into heavy Clean Architecture, generic layer folders, or artificial DDD modeling.

## Canonical Structure

Use this physical structure as the default shape:

```txt
Vertical Slice module
│
├── DependencyInjection
│   └── <ModuleName>Module
│
├── Endpoints
│   └── <ModuleName>Endpoints
│       ├── #region <AggregateName>
│       ├── feature endpoint mappings
│       └── #endregion
│
├── Features
│   │
│   ├── <AggregateName>
│   │   │
│   │   ├── <UseCaseName>
│   │   │   ├── <UseCaseName>Endpoint
│   │   │   ├── <UseCaseName>Command
│   │   │   ├── <UseCaseName>Validator
│   │   │   ├── <UseCaseName>Handler
│   │   │   ├── <UseCaseName>Response
│   │   │   └── <UseCaseName>Errors
│   │   │
│   │   └── <AnotherUseCaseName>
│   │       ├── <AnotherUseCaseName>Endpoint
│   │       ├── <AnotherUseCaseName>Command
│   │       ├── <AnotherUseCaseName>Validator
│   │       ├── <AnotherUseCaseName>Handler
│   │       ├── <AnotherUseCaseName>Response
│   │       └── <AnotherUseCaseName>Errors
│
├── Domain
│   │
│   ├── Aggregates
│   │   │
│   │   ├── <AggregateName>
│   │   │   ├── <AggregateName>
│   │   │   ├── <AggregateName>Id
│   │   │   ├── <AggregateName>Status
│   │   │   ├── <AggregateName>Events
│   │   │   └── <AggregateName>Errors
│   │   │
│   │   └── <AnotherAggregateName>
│   │       ├── <AnotherAggregateName>
│   │       ├── <AnotherAggregateName>Id
│   │       ├── <AnotherAggregateName>Status
│   │       ├── <AnotherAggregateName>Events
│   │       └── <AnotherAggregateName>Errors
│   │
│   ├── ValueObjects
│   │   └── shared value objects used by more than one aggregate
│   │
│   ├── Policies
│   │   └── domain policies with real business rules
│   │
│   └── Ports
│       └── only domain-required infrastructure or external capability abstractions
│
└── Infrastructure
    ├── Persistence
    ├── Repositories
    ├── BackgroundJobs
    └── Integrations
```

Prefer `Features/<AggregateName>/<UseCaseName>` over `Features/<UseCaseName>` so a reader can see which aggregate each use case primarily operates on.

Do not create global folders such as `Commands/`, `Handlers/`, `Validators/`, or `Responses/` for Vertical Slice modules.

## Endpoint Aggregator Regions

Use C# `#region` only in the module endpoint aggregator file to group mappings by aggregate.

Example:

```csharp
public static class ModuleEndpoints
{
    public static IEndpointRouteBuilder MapModuleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/module");

        #region AggregateName

        group.MapUseCaseEndpoint();

        #endregion

        return app;
    }
}
```

Use `#region` for readability in endpoint aggregation only. Do not use regions to hide complexity in handlers, domain objects, or infrastructure.

## Feature File Roles

Keep each use case self-contained:

- `Endpoint`: map route, bind request, call handler, translate `Result` to HTTP.
- `Command`: represent use-case input.
- `Validator`: validate input and request-level rules.
- `Handler`: orchestrate application flow, load aggregates, call domain behavior, use ports, persist, publish events.
- `Response`: define public output contract.
- `Errors`: define expected business/application failures with the project's error contract; use `E7.EasyResult` only when adopted.

Endpoint must not contain business rules.

Handler may coordinate multiple aggregates, but aggregate relationships stay by id.

## Aggregate Boundaries

Apply DDD aggregate boundaries conservatively:

- Aggregates relate to other aggregates by id, not object reference.
- Aggregate-to-aggregate coordination happens in the handler/application flow.
- Cross-aggregate coordination uses ids plus handler/application flow.
- Cross-module communication uses the provider module's `Public/` contracts or host composition, not consumer-owned ports.
- Infrastructure and external communication uses ports.
- Aggregates must not depend on repositories, EF Core, SDKs, HTTP clients, queues, or external services.
- Aggregates should expose behavior methods for internal invariants and state transitions.

Inside an aggregate, avoid unnecessary interfaces, factories, providers, strategies, and services. Direct methods are preferred when the rule is internal and has no real variation.

## Ports Rule

Default mode stays lean. Create a port only when one of these is true:

- infrastructure or external dependency is involved, such as database, Redis, AI, WhatsApp, e-mail, queues, storage, provider SDKs, or external APIs;
- a domain or application rule needs an infrastructure or external capability;
- a technical provider has real volatility or multiple implementations are expected.

Do not create ports for ordinary internal aggregate behavior, aggregate-to-aggregate coordination, or module-to-module
calls. Module-to-module contracts belong to the provider module's `Public/` surface when direct dependency is allowed, or
to host composition when the modules should not reference each other.

## Ports And Adapters Variant

Use this variant only when the user invokes `vertical-slice ports-and-adapters`, explicitly asks for Ports and Adapters/Hexagonal style, or the repository instruction assigns it to the module.

```txt
Vertical Slice module + ports-and-adapters
|
├── Features
│   └── <FeatureArea>
│       └── <UseCaseName>
│           ├── <UseCaseName>Endpoint
│           ├── <UseCaseName>Command
│           ├── <UseCaseName>Validator
│           ├── <UseCaseName>Handler
│           ├── <UseCaseName>Response
│           ├── <UseCaseName>Errors
│           └── Ports
│               └── I<UseCaseDependency>Port.cs
│
├── Domain
│   ├── Aggregates
│   ├── Policies
│   ├── ValueObjects
│   └── Ports
│       └── I<DomainCapability>Port.cs
│
└── Infrastructure
    ├── Persistence
    ├── Integrations
    ├── Messaging
    ├── BackgroundJobs
    └── <AdapterArea>
        └── <ConcreteAdapter>.cs
```

Rules for this variant:

- Everything consumed from `Infrastructure` must be behind a port.
- Feature/application ports live inside the owning use case folder: `Features/<FeatureArea>/<UseCaseName>/Ports`.
- Domain ports live in `Domain/Ports` when a domain rule needs an external capability.
- Infrastructure implements ports; it does not own application or domain contracts.
- Provider SDKs, EF Core, queues, HTTP clients, and external-system details stay inside infrastructure adapters.
- Handlers and domain code must not depend on concrete infrastructure types.
- Do not use domain ports for application-only orchestration dependencies.
- Do not let the variant create ports for in-memory internal behavior, aggregate-to-aggregate coordination, or
  module-to-module calls with no infrastructure, external system, or real technical provider volatility.

## Anti-Over-Engineering Rule

Before adding an abstraction, check:

```txt
Is this infrastructure/external communication?
Does this need a database, Redis, AI, WhatsApp, e-mail, queue, storage, provider SDK, or external API?
Are multiple technical provider implementations real now, not imaginary?
Is this only aggregate coordination or module-to-module communication that should use ids/application flow or `Public/`?
```

If the infrastructure/external/provider answers are no, do not add a port.

Prefer:

```txt
internal aggregate rule -> direct method
cross aggregate flow -> ids + handler coordination
module-to-module contract -> provider Public/ surface or host composition
infrastructure/external capability -> port
```

## Acceptance Criteria

A Vertical Slice module is well structured when:

- each use case can be understood by opening one feature folder;
- feature folders are grouped by aggregate;
- endpoint, command, validator, handler, response, and errors stay close together;
- endpoint aggregation is grouped with `#region <AggregateName>`;
- domain folders contain only real aggregates, value objects, policies, ports, and domain errors/events;
- expected failures follow the project's error contract (`E7.EasyResult` only when adopted);
- modules do not depend on the API host/composition-root project;
- domain does not depend on infrastructure;
- abstractions exist only where the boundary or volatility justifies them.
- in `ports-and-adapters` mode, every Infrastructure service consumed by Features or Domain implements a port.
- ports live with their consumer: use-case ports in Features, domain-required ports in Domain.
- Infrastructure does not own application or domain contracts.
- provider SDKs stay inside Infrastructure adapters.
- handlers and domain code do not depend on concrete Infrastructure types.


