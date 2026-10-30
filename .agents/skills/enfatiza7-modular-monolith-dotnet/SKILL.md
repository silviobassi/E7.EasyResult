---
name: enfatiza7-modular-monolith-dotnet
description: Enfatiza7 guidance for .NET modular monolith boundaries, composition, module integration, and relevant architecture checks. Use only for .NET projects using this architecture.
---

# Enfatiza7 Modular Monolith Architecture for .NET

Use this skill to keep modular monoliths explicit, lean, and auditable.

Load only the reference needed for the task:

- `composition-root`: read `references/composition-root.md`.
- `module-boundaries`: read `references/module-boundaries.md`.
- `inter-module-integrations`: read `references/inter-module-integrations.md`.
- `dependency-rules`: read `references/dependency-rules.md`.
- `architecture-tests`: read `references/architecture-tests.md`.

## Defaults

- Treat the host/API project as the composition root, not as an application layer.
- Keep `Program.cs` thin: bootstrap, middleware, module registration, endpoint mapping, migrations, and high-level calls only.
- Keep inter-module integrations out of the host/API composition root unless repository rules explicitly require host-owned orchestration.
- Keep endpoints in the owning module when the repository style allows module-owned endpoints.
- Use ports for infrastructure and external capabilities such as databases, Redis, AI, WhatsApp, e-mail, queues, storage, provider SDKs, and similar technical boundaries.
- Use the provider module's `Public/` surface for direct module-to-module contracts.
- Let provider modules expose minimal public contracts for capabilities they intentionally share; prefer capability-specific `I<Capability>Provider` names over broad `I<ModuleName>Service` names.
- Put every module, bounded-context, or aggregate boundary surface under module-root `Public/`.
- Treat everything under `Public/` as intentionally `public`; non-boundary implementation stays `internal` by default.
- Keep implementations of `Public` interfaces inside the owning module's normal architecture folders.
- Register only public boundary contract bindings through `Public/DependencyInjection`; keep full module bootstrap and internal dependency injection outside `Public/DependencyInjection`.
- Perform provider-to-consumer model translation inside the consumer module adapter/application flow unless repository rules explicitly require host-owned orchestration.
- Keep module dependencies explicit: direct module-to-module references are allowed only to the provider module's `Public/` surface, or to an approved contracts package when repository rules define one.
- Keep implementation types `internal` by default; make types `public` only for intentional boundaries.
- Run relevant existing architecture tests when a protected boundary changes. Add a new one only for an explicit rule with current value that existing tools do not cover.
- Each bounded context owns and validates its configuration. Shared infrastructure configuration has an explicit owner; a context does not need its own file or database.

## Decision Rule

Before creating a port, adapter, public contract, or shared package, ask:

```txt
Does this cross a module/bounded-context boundary that should be exposed through `Public/`?
Does this cross the host/API, frontend, package, or external-system boundary?
Does a domain rule need an infrastructure/external capability?
Is there real provider volatility or multiple implementations now?
```

If all answers are no, prefer a direct internal dependency inside the same module.

## Output Expectations

When proposing structure, name folders, files, classes, and extension methods explicitly.
When reviewing code, call out boundary leaks first, then naming/organization issues.
When repository instructions conflict with this skill, follow the repository instructions and use this skill for the gaps.
