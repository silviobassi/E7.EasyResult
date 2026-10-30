
# Enfatiza7 CQRS Event Sourcing Structure

Use this skill when an Enfatiza7 backend module needs CQRS with Event Sourcing and Projections.

This pattern is for domains where history is the source of truth. Do not use it for simple CRUD, administrative settings, or procedural flows.

## When To Use

Use CQRS + Event Sourcing when:

- the domain needs complete history;
- state must be rebuilt from events;
- auditability is central;
- multiple read models or projections are needed;
- timeline, replay, reporting, AI/RAG context, or analytics depend on event history;
- business behavior belongs in an aggregate;
- persisted facts matter more than current-table state.

Do not use it when:

- a single current-state table is enough;
- the use case is simple CRUD;
- the module has no meaningful aggregate behavior;
- projections would only duplicate one screen;
- event sourcing is being added only for architecture style.

## Canonical Structure

Use this structure by default:

```txt
CQRS Event Sourcing module
│
├── DependencyInjection
│   └── <ModuleName>Module
│
├── Endpoints
│   ├── <WriteResource>Endpoints
│   └── <ReadResource>Endpoints
│
├── WriteModel
│   ├── Commands
│   ├── CommandHandlers
│   └── EventSourcing
│
├── ReadModel
│   ├── Queries
│   ├── QueryHandlers
│   ├── Projections
│   └── ReadModels
│
├── Domain
│   ├── <AggregateName>
│   │   ├── <AggregateName>
│   │   ├── <AggregateName>State
│   │   └── <AggregateName>Events
│   ├── ValueObjects
│   ├── Errors
│   └── EventSourcing
│       ├── AggregateRoot
│       ├── DomainEvent
│       └── EventMetadata
│
└── Infrastructure
    ├── EventStore
    ├── Snapshots
    └── Projections
```

Prefer fewer folders when the module is small. Add more only when the code needs the separation.

## Write Side Flow

Write endpoints must send commands to the write model:

```txt
Write Endpoint
│
└── Command Handler
    │
    ├── validate command
    ├── load stream from event store
    ├── rebuild aggregate by replaying events
    ├── execute aggregate command
    ├── append new uncommitted events
    ├── update projections when appropriate
    └── return Result/Response
```

Commands change state only by producing events.

Do not persist current aggregate state as the source of truth.

## Read Side Flow

Read endpoints must query read models or projections:

```txt
Read Endpoint
│
└── Query Handler
    │
    ├── read projection/read model
    ├── apply filters/paging
    └── return response
```

Queries must not rebuild aggregates for normal reads.

Use aggregate replay for command decisions, recovery, repair, migration, or projection rebuilds.

## Domain Modeling Rules

Use a real domain model here, but keep it lean.

The aggregate should:

- expose behavior through public command methods;
- protect invariants;
- hold uncommitted events;
- rebuild itself from historical events;
- apply new events through the same state transition path used by replay;
- avoid public setters for domain state.
- dispatch events explicitly with switch/pattern matching in When; do not use dynamic for new event-sourced modules.

The state object should:

- stay internal to the aggregate;
- hold mutable current state;
- change only through `Apply` methods;
- be rebuilt from event history.

The event model should:

- represent facts that already happened;
- be immutable;
- be suitable for serialization and versioning;
- carry tenant/correlation metadata directly or through metadata;
- avoid C# type-style decisions that belong to a future code style guide.

Do not make event-store domain events `struct` by default. They need serialization, versioning, metadata, and schema evolution.

## Replay Rule

Historical replay must not add events to `UncommittedEvents`.

Use this conceptual split:

```txt
Append new event -> apply state change + track as uncommitted
Replay old event -> apply state change only
```

Never publish or persist historical events again during replay.

Implement AggregateRoot with separate paths for new events and replay:

~~~csharp
protected void AppendEvent(IDomainEvent @event)
{
    ApplyChange(@event);
    _uncommittedEvents.Add(@event);
}

public override void LoadFromHistory(IEnumerable<IDomainEvent> events)
{
    foreach (var @event in events)
    {
        ApplyChange(@event);
    }
}

private void ApplyChange(IDomainEvent @event)
{
    When(@event);
}
~~~

Implement aggregate When with explicit event dispatch:

~~~csharp
protected override void When(IDomainEvent @event)
{
    switch (@event)
    {
        case AddedFaithHistoryEvent e:
            _state.Apply(e);
            break;
        case ChangedReligionEvent e:
            _state.Apply(e);
            break;
        default:
            throw new InvalidOperationException(nameof(IDomainEvent));
    }
}
~~~

Keep the existing domain folder and type naming style when modernizing older modules: AggregateRoot, EventSourcingLoader, Commands, Events, States, and Projections.

## Expected failure rules

Use the project's error contract for expected business failures; use `E7.EasyResult` only when adopted.

Aggregate public commands should express expected rule failures through the project's declared error contract; `Result` or `Result<T>` is appropriate when that contract is adopted.

Examples:

- aggregate not found;
- already closed;
- invalid transition;
- participant not found;
- message text required;
- duplicate event/business action;
- forbidden command for current state.

Only append a new event after the business rule succeeds.

Keep the project's expected business-flow contract in event-sourced modules. Do not replace expected failures with exceptions just because the aggregate is event-sourced.

Preferred command flow:

~~~txt
validate command/business rule -> Result failure? return it
success -> create domain event -> AppendEvent(event) -> return Result.Success
~~~

Aggregate command methods may return Result or Result<T> directly, or handlers may translate validation/domain failures into Result. In both cases, never append an event after a failed Result.

Use exceptions for technical failures:

- event store outage;
- serialization failure;
- projection storage failure;
- timeout;
- programming bug.

Do not throw exceptions for normal business branching.

## Event And Projection Rules

Domain events are internal facts of the module.

Integration events are stable contracts for other modules or external systems. When they cross the module boundary,
place them under the module's `Public/Events` surface.

Do not expose internal event-store event classes as external integration contracts unless that is an explicit boundary decision.

Projections should:

- be derived from events;
- be rebuildable from the event store;
- serve real read use cases;
- avoid becoming a second write model;
- stay eventually consistent unless the use case requires synchronous consistency.
- use explicit switch/pattern matching dispatch for new code; avoid dynamic dispatch because it hides unsupported events and adds runtime binder overhead.

Create only projections that have a current consumer.

Domain-level projections may follow the existing EventSourcingLoader shape:

~~~csharp
public sealed class ReligiousChangesTimelineProjection : EventSourcingLoader
{
    public int Version { get; private set; } = -1;

    public override void LoadFromHistory(IEnumerable<IDomainEvent> events)
    {
        foreach (var @event in events)
        {
            When(@event);
        }
    }

    protected override void When(IDomainEvent @event)
    {
        switch (@event)
        {
            case ChangedReligionEvent e:
                Apply(e);
                break;
            case ChangedReligiousInstitutionEvent e:
                Apply(e);
                break;
            default:
                throw new InvalidOperationException(nameof(IDomainEvent));
        }

        Version++;
    }

    private void Apply(ChangedReligionEvent e)
    {
        // update projection state
    }
}
~~~

## Snapshot Rule

Snapshots are optional.

Do not create snapshot infrastructure at the start unless:

- streams are already expected to be large;
- replay time is a measured problem;
- a product workflow requires fast reconstruction from long histories.

If snapshots are added, they are an optimization. Events remain the source of truth.

## Abstractions And Ports Rule

Do not create ports between internal layers.

Create abstractions only for infrastructure, transversal concerns, or external boundaries.

Allowed abstractions:

- event store;
- projection storage;
- snapshot storage;
- external publisher/event bus;
- clock/current tenant/current user;
- external provider;

For cross-bounded-context contracts, use the provider module's `Public/` surface when direct dependency is allowed, or
compose through the host. Do not create a consumer-owned port just to call another module.

Avoid abstractions for:

- commands;
- queries;
- handlers;
- internal projectors with one implementation;
- simple mappers;
- hypothetical event store providers;
- internal calls in the same module.

Repository-style interfaces are allowed only when they protect infrastructure or improve testing clearly. They are not mandatory.

## Endpoint Rules

Endpoints should:

- map routes;
- bind requests;
- call one command or query handler;
- translate `Result` to HTTP;
- stay thin.

Endpoints should not contain business rules, replay logic, projection logic, or event-store details.

## Pattern Selection

Use this quick check before adding Event Sourcing:

```txt
Simple procedural flow? -> Transaction Script
Record has lifecycle behavior? -> Active Record
Rules are volatile per use case? -> Vertical Slice
Rich invariants without event history? -> Domain Model
Event history/rebuild/projections? -> CQRS/Event Sourcing
```

If history is not valuable, do not use Event Sourcing.

## Anti-Over-Engineering Rule

Do not create:

- event sourcing for CRUD;
- snapshot infrastructure by default;
- separate read database by default;
- internal event bus just for local method calls;
- projection per screen without reuse or need;
- generic handler interfaces without project convention;
- factories/specifications/services with no domain pressure;
- ports for internal layers;
- event-versioning framework before versioning pain exists.

Prefer a small event-sourced core: aggregate, state, events, event store, command handlers, query handlers, projections.

## Acceptance Criteria

A CQRS Event Sourcing module is well structured when:

- write endpoints go through commands;
- read endpoints go through queries/read models/projections;
- aggregate state can be rebuilt from events;
- replay does not create uncommitted events;
- domain events are immutable and versionable;
- `State` is separate from aggregate behavior;
- business failures follow the project's error contract (`E7.EasyResult` only when adopted);
- technical failures use exceptions/logging/resilience;
- projections are rebuildable;
- snapshots are optional optimizations;
- infrastructure details stay in `Infrastructure`;
- abstractions exist only for infrastructure, transversal concerns, or external boundaries;
- no ceremony exists without domain pressure.

