# Module Boundaries

Use when deciding what belongs inside a module, what crosses a boundary, and which types may be public.

## Ownership

Each module owns its business state, rules, persistence, and internal implementation.

A module should expose only intentional boundaries:

- endpoint contracts;
- integration events;
- public application contracts needed by another module;
- stable shared abstractions approved by repository rules.

Do not expose internal domain, persistence, handlers, validators, or provider SDK details just because another module wants convenience access.

## Public Folder

Every contract, interface, event, dependency-injection extension, or model that crosses a module, bounded-context,
aggregate, host, frontend, package, or external-system boundary must live under module-root `Public/`.

Everything under `Public/` is intentionally `public`. Implementation remains inside the owning module's normal
architecture folders and follows the module's adopted pattern.

Use this shape when applicable:

```txt
<ModuleName>/
  Public/
    Interfaces/
    Contracts/
      Requests/
      Responses/
      Dtos/
      Records/
      Enums/
      Errors/
    Events/
    DependencyInjection/
```

Public dependency-injection extension methods live in `Public/DependencyInjection` only when they register public boundary contracts. These extensions may bind public interfaces to internal implementations from the same module.

Do not move full module bootstrap, internal dependency-injection modules, persistence registration, options, endpoint mapping, migrations, workers, or internal-only services into `Public/DependencyInjection`. Keep those registrations in the module-internal dependency-injection/bootstrap structure according to the local architecture.

Use `Public/Events` only for events that cross the module boundary asynchronously or represent a notification between
modules. Do not put internal domain events under `Public/Events` unless they are intentionally published outside the
owning module.

## Consumer And Provider Rule

For inter-module behavior:

```txt
Consumer module defines what it needs.
Provider module owns how the capability is fulfilled.
Consumer invokes the provider module's `Public/` surface directly when the dependency is allowed.
```

The consumer should not know the provider module's internal model. The provider should not know the consumer workflow. Name public interfaces by capability, preferring `I<Capability>Provider` over broad `I<ModuleName>Service` names.

## Ports

Create a port only when the consumer crosses an infrastructure or external-system boundary:

- database;
- Redis or cache;
- AI provider;
- WhatsApp, e-mail, SMS, or other communication provider;
- queues, streams, or background messaging infrastructure;
- storage provider;
- other provider SDK, external API, or technical infrastructure;
- infrastructure capability needed by domain/application code;
- real provider volatility or multiple implementations.

Do not create a port merely to call another module. For module-to-module behavior, depend on the provider module's
`Public/` surface when direct dependency is allowed. Use host-owned orchestration only when repository rules explicitly require it.

Do not create ports for:

- same-module repository calls by default;
- simple validators;
- one-line calculations;
- internal domain behavior;
- hypothetical future providers.

## Public Types

Keep implementation types `internal` by default.

Use `public` only for:

- stable contracts intentionally consumed outside the assembly;
- module registration extensions used by the host;
- endpoint request/response contracts when the repository exposes them from module assemblies;
- integration events and shared abstractions explicitly crossing boundaries.

Public is a boundary decision, not a convenience modifier.
