---
name: enfatiza7-backend-architecture-dotnet
description: Route Enfatiza7 .NET backend architecture guidance for modular-monolith modules, Vertical Slice, Transaction Script, Active Record, CQRS, and test strategy. Use only for matching .NET backend work.
---

# Enfatiza7 Backend Architecture for .NET

Use this skill as a router. Load only the reference files needed for the requested architecture or practice.

## Command Routing

- `vertical-slice`: read `references/vertical-slice.md`.
- `vertical-slice ports-and-adapters`: read `references/vertical-slice.md` and apply its `ports-and-adapters` variant.
- `transaction-script`: read `references/transaction-script.md`.
- `active-record`: read `references/active-record.md`.
- `cqrs-event-sourcing`: read `references/cqrs-event-sourcing.md`.
- `cqrs-read-models`: read `references/cqrs-read-models.md`.
- `csharp-dotnet-coding-style`: read `references/csharp-dotnet-coding-style.md`.
- `dotnet-test-architecture`: read `references/dotnet-test-architecture.md`.
- `configuration-by-bc`: read `references/configuration-by-bc.md` when a .NET bounded context's options or their ownership change.

## Defaults

- Prefer the architecture style already assigned to the bounded context.
- Keep implementation types `internal` unless they intentionally cross a boundary.
- Use `E7.EasyResult` for expected business failures only when the project has adopted that library; otherwise follow its declared result/error convention.
- Avoid artificial ports, interfaces, rich domain models, or generic layers unless the selected reference explicitly requires them.
- Keep provider SDKs, EF Core, HTTP clients, queues, and external systems out of domain code.
- For projects organized by bounded context, keep typed configuration owned and validated by the consuming context. The host assembles sources and shared configuration has an explicit owner. A separate configuration file or database per context is not required.
- Treat the reference structures as profiles, not mandatory layers. Use strategic DDD for boundaries and tactical DDD only where invariants justify a rich domain model.
- Apply any `E7.EasyResult` and library-specific examples in the references only if the consuming project uses those libraries.

## Ambiguity

If the user names a bounded context but not an architecture command, infer the command from repository instructions or existing module structure. If several commands fit equally well, ask one concise question before reorganizing structure.
