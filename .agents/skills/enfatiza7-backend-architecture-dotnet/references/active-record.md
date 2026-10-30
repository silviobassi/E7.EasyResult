
# Enfatiza7 Active Record Structure

Use this skill when an Enfatiza7 backend module follows pragmatic Active Record architecture.

This pattern fits records with identity, persistence, lifecycle state, and simple local rules. It is stronger than DTO-only CRUD and lighter than rich Domain Model/Clean Architecture.

## When To Use

Use pragmatic Active Record when:

- the module manages persistent records with lifecycle state;
- local transitions and validations belong near the record;
- rules are not complex enough for a rich domain model;
- Application Services still need to orchestrate transactions, external providers, webhooks, secrets, and events.

Common examples:

- billing, subscriptions, invoices, payment attempts, entitlements;
- external integrations, webhooks, provider configuration, phone numbers;
- API applications, API credentials, API keys, scopes, revocation.

## Canonical Structure

Use this physical structure as the default shape:

```txt
Active Record module
│
├── DependencyInjection
│   └── <ModuleName>Module
│
├── Endpoints
│   ├── <ResourceName>Endpoints
│   ├── <AnotherResourceName>Endpoints
│   └── <WebhookOrOperation>Endpoints
│
├── Application
│   ├── Commands
│   ├── Queries
│   └── Services
│
├── Domain
│   ├── ActiveRecords
│   │   ├── <ActiveRecordName>
│   │   └── <ChildRecordName>
│   │
│   ├── Enums
│   ├── ValueObjects
│   └── Errors
│
├── Infrastructure
│   ├── Persistence
│   ├── Providers
│   ├── Secrets
│   ├── Webhooks
│   └── ExternalAdapters
│
└── Contracts
```

Do not add `Application/Ports` by default. Do not create ports between internal layers. Add ports only when there is
transversal communication, infrastructure dependency, or an external-system boundary.

Default dependency flow:

```txt
Endpoint -> Application Service -> Active Record
Application Service -> Infrastructure
```

Use direct internal references between module layers unless an external boundary must be protected.

## Active Record Shape

Model active records as persistent objects with local behavior:

```txt
<ActiveRecordName>
│
├── Identity
├── TenantId
├── Status
├── Local configuration
├── Child records / owned records
├── Local state transitions
└── Local validation rules
```

Example conceptual composition:

```txt
IntegrationRecord
│
├── Provider Application
├── Business Account
├── Webhook
└── Phone Numbers
```

Use real provider/business names in actual projects when they are known. Do not create generic provider abstractions too early.

## Allowed Inside Active Records

Active records may contain:

- local validations;
- local invariants;
- lifecycle methods;
- status transitions;
- simple calculations;
- child/owned record management;
- one-default-item rules;
- token or secret references, not raw secrets;
- `Result` / `Result<T>` for expected business failures.

Examples:

```txt
record.Activate(...)
record.Disable(...)
record.Cancel(...)
record.MarkAsPaid(...)
record.MarkAsOverdue(...)
record.AddChild(...)
record.RemoveChild(...)
record.SetDefaultChild(...)
record.RotateTokenReference(...)
```

## Not Allowed Inside Active Records

Active records must not directly:

- call external APIs;
- call payment gateways;
- call provider SDKs;
- call Secret Provider;
- publish to event bus;
- send email/messages;
- access external queues;
- call another bounded context;
- use EF Core directly;
- create or validate hashes through infrastructure;
- store raw tokens, API keys, app secrets, or verify tokens.

These operations belong in Application Services plus Infrastructure adapters.

## Application Service Role

Application Services orchestrate use cases:

```txt
Application Service
│
├── load active records
├── call active record methods
├── coordinate transactions
├── call external-boundary/infrastructure ports when needed
├── persist changes
├── publish integration events
├── process webhooks
└── build responses/read models
```

Keep Application Services procedural and explicit. Do not push orchestration into the active record.

## External Boundary Ports Rule

Do not create abstractions or ports between internal layers.

Create abstractions or ports only when the dependency is infrastructure-facing, transversal, or crosses an
external-system boundary.

Create ports only for:

- external providers;
- secret storage;
- token/hash generation;
- payment gateways;
- email/message providers;
- event bus or external queue;
- security validation contracts;
- clock/current user/current tenant if shared across modules.

For cross-module or bounded-context contracts, use the provider module's `Public/` surface when direct dependency is
allowed, or compose through the host. Do not create a consumer-owned port just to call another module.

Do not create ports for:

- active record methods;
- simple local calculations;
- basic validators;
- ordinary repository calls inside the same module when the project already uses direct infrastructure registration;
- hypothetical future providers with no current product need.

Repository guidance:

```txt
Simplest acceptable style:
Application Service -> concrete Infrastructure repository

More testable style:
Application Service -> repository interface
Infrastructure -> repository implementation
```

Choose the repository interface only when it improves testing, DI, or boundary clarity. Do not treat repository interfaces as mandatory ceremony.

## Anti-Over-Engineering Rule

Do not create a rich Domain Model when local Active Record behavior is enough.

Do not create generic records such as:

```txt
Integration
Provider
JsonConfiguration
GenericCredential
UniversalWebhook
```

unless the product already has proven cross-provider behavior and stable shared rules.

Prefer one meaningful active record per real business object/provider.

Decision guide:

```txt
Local state rule? -> ActiveRecord method
Use case orchestration? -> Application Service
Infrastructure/external dependency? -> boundary port + Infrastructure adapter
Public HTTP? -> Endpoint
Stable external contract? -> Contracts
```

## Errors And Security

Use the project's error contract for expected business failures (`E7.EasyResult` only when adopted):

- already canceled;
- already paid;
- inactive default child;
- missing required provider configuration;
- duplicate credential;
- expired or revoked credential;
- usage limit reached;
- invalid local state transition.

Use exceptions for technical failures:

- database outage;
- network timeout;
- provider unavailable;
- unexpected serialization failure;
- programming bug.

Never expose raw secrets, tokens, or API keys in:

- domain events;
- integration events;
- logs;
- read models;
- responses;
- admin screens.

Persist references or hashes only.

## Acceptance Criteria

An Active Record module is well structured when:

- it has clear `Endpoints`, `Application`, `Domain/ActiveRecords`, `Infrastructure`, and `Contracts` boundaries;
- active records contain state and simple local behavior;
- Application Services orchestrate transactions and external work;
- infrastructure adapters contain provider-specific details;
- ports do not exist between internal layers;
- ports exist only for infrastructure, transversal, or external-boundary communication;
- expected business failures follow the project's error contract (`E7.EasyResult` only when adopted);
- raw secrets and API keys never leak outside secure infrastructure;
- the module avoids generic provider/configuration abstractions until the product proves they are needed.

