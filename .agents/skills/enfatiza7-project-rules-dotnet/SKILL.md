---
name: enfatiza7-project-rules-dotnet
description: Enfatiza7 reusable project rules for .NET repositories. Use when applying local architecture, specification, testing, documentation, and configuration conventions without importing Traction7-specific details.
---

# Enfatiza7 Project Rules for .NET

Read the repository's `AGENTS.md` and `.harness/project-profile.md` for its actual stack, architecture, commands, and bounded-context map. This skill generalizes reusable guidance from Traction7; it does not carry Traction7 module names, directories, persistence choices, or approved provider decisions into another project.

## Scope and context

- Respect the user's task type. Do not implement code for a specification-only or review-only request.
- Inspect the affected module first. Read other modules only when contracts, shared code, integrations, or project-wide decisions make them relevant.
- Treat implemented code as evidence of current behavior; consult documentation for declared decisions and keep it current when the task changes those decisions.
- Preserve project-specific persistence, naming, and tool choices. Do not import local shell requirements from another repository.

## Architecture and configuration

- Use the architecture declared for each bounded context. Strategic DDD can clarify boundaries; tactical DDD applies only where invariants justify it.
- Keep module contracts explicit and prevent consumers from reaching into another module's internals. Load `enfatiza7-modular-monolith-dotnet` only when that architecture applies.
- Each context owns and validates its options, including local resilience settings. Shared infrastructure has an explicit configuration owner. See `enfatiza7-backend-architecture-dotnet` for .NET binding guidance when needed.
- Use `E7.EasyResult` only when the project has adopted it. Keep expected business failures distinct from unexpected technical failures.

## Specs and verification

- For any SDD workflow, use the four-digit sequential spec folder rule in `AGENTS.md`, independent of the selected tool.
- Follow the project's chosen test shape; TDD is optional. Use focused tests for local changes, broader relevant checks for shared or boundary changes, and one project build at the end of the implementation scope.
- Integration tests using a real database must clean up only data they created, using rollback or reliable cleanup where appropriate.
- Architecture tests protect explicit, valuable rules; do not add one automatically for every new type or module.
