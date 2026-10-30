
# Enfatiza7 CQRS Read Models And Projections

Use this skill when an Enfatiza7 backend module is an optimized read side.

This pattern is for reporting, dashboards, metrics, analytics, search/listing screens, and read APIs derived from other sources.

Reporting reads and materializes views. It does not own transactional business decisions.

## When To Use

Use CQRS read models and projections when:

- a screen or report needs optimized read data;
- data comes from multiple modules, contexts, tables, events, or external sources;
- denormalized models make reads simpler or faster;
- dashboards need aggregates, counters, metrics, or timelines;
- projections can be rebuilt from source data;
- the module answers questions instead of changing core business state.

Do not use this pattern when:

- a direct query is enough;
- the module owns transactional invariants;
- there is rich aggregate behavior;
- the flow is a simple command/procedure;
- the code is CRUD administration.

## Canonical Structure

Use this structure by default:

```txt
CQRS Read Models / Projections module
│
├── DependencyInjection
│   └── <ModuleName>Module
│
├── Endpoints
│   ├── <DashboardName>Endpoints
│   ├── <ReportName>Endpoints
│   └── <LookupName>Endpoints
│
├── Queries
│   ├── <QueryName>
│   │   ├── <QueryName>Query
│   │   ├── <QueryName>Validator
│   │   ├── <QueryName>Handler
│   │   ├── <QueryName>Response
│   │   └── <QueryName>Errors
│   │
│   └── ...
│
├── Projections
│   ├── <ProjectionName>
│   │   ├── <ProjectionName>Projector
│   │   ├── <ProjectionName>ReadModel
│   │   └── <ProjectionName>Checkpoint
│   │
│   └── ...
│
├── ReadModels
│   ├── <ReadModelName>
│   └── ...
│
└── Infrastructure
    ├── Persistence
    ├── ProjectionStorage
    ├── EventConsumers
    └── ExternalSources
```

Prefer fewer folders when the module is small. Add projection/checkpoint folders only when projections actually exist.

Do not create `Domain/` by default.

## Default Flow

Read endpoints should use query handlers:

```txt
Endpoint
│
└── Query Handler
    │
    ├── validate query/filter
    ├── read read model/projection
    ├── apply paging/sorting/filtering
    └── return response
```

Projection flow:

```txt
Source event/data change
│
└── Projector
    │
    ├── read event/source payload
    ├── update denormalized read model
    ├── save checkpoint when needed
    └── keep projection rebuildable
```

## Reporting Ownership Rules

Reporting modules should:

- own read models;
- own projection code;
- own query endpoints;
- own dashboard/report response shapes;
- tolerate denormalized and duplicated read data;
- optimize for read clarity and performance.

Reporting modules should not:

- own transactional business rules from another module;
- mutate core domain state;
- call aggregates from other modules;
- become the source of truth for operational state;
- expose internal projections as business commands.

Read models are disposable. Source data is not.

## Query Rules

Queries should:

- represent a read use case;
- accept filters, paging, sorting, tenant, and user context as needed;
- return a response optimized for the consumer;
- avoid leaking persistence entities when a response model is clearer.

Queries may read:

- projection tables;
- materialized views;
- read-optimized tables;
- cached snapshots;
- external read APIs;
- direct SQL joins when no projection is needed.

Do not create a projection if one query against existing data is enough.

## Projection Rules

Create projections only when they serve a real read use case.

Good reasons for a projection:

- expensive joins repeated often;
- cross-context read model;
- dashboard counters;
- timeline/read history;
- analytics by period/status/user;
- AI/RAG context read model;
- mobile/tablet optimized read model;
- eventual consistency is acceptable.

Projection code should be deterministic and rebuildable.

Store checkpoints only when consuming ordered streams, queues, event logs, or long-running rebuilds.

Do not create a projector per screen by habit.

For event-sourced domain projections, keep the existing domain naming and folder shape when present (Projections/<ProjectionName>, EventSourcingLoader, IDomainEvent), but dispatch events with explicit switch/pattern matching instead of dynamic.

~~~csharp
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
~~~

Do not let projection replay append uncommitted events. Projection replay should call When/Apply only and update projection state.

## Read Model Rules

Read models may be:

- denormalized;
- duplicated;
- shaped for one screen/report;
- indexed for query speed;
- stored separately from write tables;
- rebuilt from source data when needed.

Read models should not contain business behavior.

Use simple records/classes that describe the data needed by the read side.

## Commands And Rebuild Operations

Do not create a write model by default.

Allowed operational commands:

- rebuild projection;
- refresh materialized view;
- reprocess event range;
- repair checkpoint;
- clear cache.

Keep these commands operational, not business-transactional.

## Expected failure rules

Use the project's error contract for expected read-side failures (`E7.EasyResult` only when adopted):

- invalid date range;
- unsupported filter;
- missing tenant access;
- report unavailable;
- projection not ready;
- requested export too large;
- read model not found when absence is expected.

Use exceptions for technical failures:

- database outage;
- timeout;
- serialization failure;
- external source unavailable;
- projection storage failure;
- programming bug.

Do not throw exceptions for normal reporting validation.

## Abstractions And Ports Rule

Do not create ports between internal query/projection layers.

Create abstractions only for infrastructure, transversal concerns, or external boundaries.

Allowed abstractions:

- external source client;
- event stream consumer;
- projection storage when it hides infrastructure;
- export/storage provider;
- clock/current tenant/current user;
- cache provider;

For cross-bounded-context data, use the provider module's `Public/` contracts/events when direct dependency or event
consumption is allowed, or compose through the host. Do not create a consumer-owned port just to call another module.

Avoid abstractions for:

- every query handler;
- simple read models;
- simple projectors with one implementation;
- internal mappers;
- one-table queries;
- hypothetical storage engines.

Direct SQL/read storage access from query handlers is acceptable when simple and aligned with the project.

## Consistency Rule

Read models are usually eventually consistent.

Make consistency explicit when it matters:

- show last updated timestamps when useful;
- use checkpoints for event streams;
- expose projection status for long rebuilds;
- prefer synchronous reads only when the workflow requires immediate consistency.

Do not force distributed transaction behavior into reporting.

## Pattern Selection

Use this quick check:

```txt
Simple procedural write flow? -> Transaction Script
Persistent record with local lifecycle? -> Active Record
Feature-local business use case? -> Vertical Slice
Event history is source of truth? -> CQRS/Event Sourcing
Optimized derived reads/reporting? -> CQRS Read Models/Projections
```

If the module starts deciding business state, move that behavior to the owning domain module.

## Anti-Over-Engineering Rule

Do not create:

- `Domain/` by default;
- aggregates;
- domain services;
- write model for reporting;
- projection when a direct query is enough;
- separate read database before need;
- generic repositories for every read model;
- ports between internal layers;
- event consumers without a real source;
- checkpoints for simple synchronous updates;
- projection rebuild framework before rebuild is needed.

Prefer the simplest read side that answers the query clearly and can evolve into projections when needed.

## Acceptance Criteria

A CQRS Read Models/Projections module is well structured when:

- endpoints are read-focused and thin;
- queries read read models, projections, or direct read storage;
- projections exist only for real consumers;
- read models are optimized for reads, not domain behavior;
- reporting does not mutate transactional business state;
- `Domain/` does not exist by default;
- expected read failures follow the project's error contract (`E7.EasyResult` only when adopted);
- technical failures use exceptions/logging/resilience;
- projection rebuild is possible when projections exist;
- abstractions exist only for infrastructure, transversal concerns, or external boundaries;
- no reporting ceremony exists without a current read-side need.

