# Dependency Rules

Use when defining allowed references between host, modules, contracts, frontend, shared kernel, and infrastructure.

## Baseline Direction

A modular monolith should usually allow:

```txt
Host/API -> Modules
Host/API -> Shared infrastructure/security/contracts
Modules -> Shared kernel/contracts/commons allowed by repo
Frontend -> Public HTTP/API contracts only
```

A modular monolith should usually forbid:

```txt
Modules -> Host/API
Frontend -> Module internals
Domain -> Infrastructure
Domain -> provider SDKs/HTTP clients/queues/EF Core
Security/shared infrastructure -> module domain internals
Module A -> Module B internals
```

If a direct module-to-module dependency is permitted, depend only on the provider module's `Public/` surface or on a
small approved contracts assembly. Never depend on provider internals.

Ports are for infrastructure and external-system dependencies, such as databases, Redis, AI providers, WhatsApp, e-mail,
queues, storage, provider SDKs, and external APIs. Module-to-module dependencies use the provider module's `Public/`
surface instead of consumer-owned ports.

## Shared Code

Create shared code only for stable cross-cutting concerns.

Good shared candidates:

- primitives and base abstractions used across modules;
- stable public contracts;
- clock/current user/current tenant abstractions;
- reusable error/result helpers already standardized by the repo.

Poor shared candidates:

- one module's persistence models;
- provider-specific SDK wrappers with one consumer;
- speculative generic abstractions;
- business rules owned by a bounded context.

## Provider SDK Rule

Provider SDKs and external API details belong in infrastructure or provider modules. Keep them out of domain code and out of unrelated modules.

If another module needs a provider-backed capability, expose a provider-module `Public/` contract and let the consumer depend on that public surface when the dependency is allowed. The host registers modules and public boundary DI extensions as needed; it should not own BC-to-BC business wiring unless repository rules explicitly require it.

## Public Dependency Injection

`Public/DependencyInjection` is only for public boundary contract registrations, such as binding a public provider interface to an internal implementation in the owning module.

Do not move full module bootstrap, internal dependency-injection modules, persistence registration, options, endpoint mapping, migrations, workers, or internal-only service registration into `Public/DependencyInjection`.

## Configuration

Module-owned configuration stays with the owning module registration. Cross-cutting configuration may live in the host only when it affects the host pipeline or multiple modules.
