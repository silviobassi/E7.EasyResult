# Repository Guidelines

## Project Structure & Module Organization

`E7.sln` contains the library and its test project:

- `src/E7.EasyResult/`: `Result`, `Result<T>`, notifications, and railway extensions such as `Bind`, `Map`, and
  `Ensure`.
- `src/E7.EasyResult/Errors/`: application errors, HTTP error types, and concrete error records.
- `tests/E7.EasyResult.Tests/`: unit tests; `Services/` and `Responses/` contain test helpers.
- `.github/workflows/production.yml`: restore, test, version tagging, packaging, and NuGet publication.
- `README.md` and `LICENSE`: usage documentation and package metadata. There is no separate application or asset
  directory.

## Build, Test, and Development Commands

Run commands from the repository root:

- `dotnet restore E7.sln`: restore dependencies.
- `dotnet build E7.sln --configuration Release`: compile the solution.
- `dotnet test E7.sln --configuration Release`: run all tests, matching CI's configuration.
- `dotnet test E7.sln --filter FullyQualifiedName~RailwayExtensionsTests`: run focused railway tests.
- `dotnet test E7.sln --collect:"XPlat Code Coverage"`: collect coverage with Coverlet.
- `dotnet pack src/E7.EasyResult/ --configuration Release -o packages`: generate a local NuGet package.

CI installs .NET `10.0.x`. Use an SDK compatible with the checked-out target frameworks, and keep library, test, and CI
targets compatible. This repository builds a library; there is no local server to start.

## Coding Style & Naming Conventions

Follow existing C# style: four-space indentation, file-scoped namespaces, and braces on separate lines. Use PascalCase
for types and public members, camelCase for parameters and locals, and `Async` suffixes for asynchronous methods. Keep
nullable reference types enabled and document public APIs with XML comments. Place new errors under `Errors/`. No
formatter or lint command is configured.

## Testing Guidelines

Use xUnit `[Fact]` tests and Shouldly assertions. Name files `<Subject>Tests.cs` and methods descriptively, following
`Ensure_Should_Return_Error_When_Predicate_Is_False`. Cover success, failure propagation, notification aggregation, and
asynchronous behavior when relevant. No coverage threshold is configured.

## Commit & Pull Request Guidelines

Recent commits use `feat:`, `refactor:`, and `docs:`, with optional scopes such as `feat(railway): add async bind`.
Follow that pattern with concise, action-oriented subjects. PRs should explain behavior changes, include test results,
link relevant issues, and update README examples for public API changes. Keep NuGet credentials in GitHub secrets;
pushing qualifying changes to `main` triggers the release workflow.

## Agent Instructions: TypeSafe

Use the installed `typesafe-ai` skill when working on this project and apply Jev as a decision model where its judgment
capabilities fit. Follow the skill's guidance: ask Jev for specific judgments and keep decision logic explicit and
testable. `TYPESAFE_API_KEY` is stored in `~/.config/ai-secrets/jev.env`; load it securely when needed, and never print
or commit the key. Follow the skill's permission and budget requirements before sending live API requests.

<!-- ENFATIZA7 HARNESS START -->

# Enfatiza7 engineering harness

This repository uses an agent-interpreted workflow. Read only the instructions, skills, code, and evidence relevant to
the current task. User instructions and repository-specific rules take precedence over harness defaults. Read
`.harness/project-profile.md` when the task needs project commands, architecture, configuration, external integrations,
test strategy, or spec locations; an unfilled field is not a prohibition.
At project setup or a meaningful tooling refresh, use the matching setup skill and `.harness/tooling-defaults.md` to
inspect existing tools, ask for owner preferences/references, and propose nominal choices before configuration changes.
Do not repeat setup research for each implementation task or re-offer declined tools without a relevant reason.

Before invoking an external integration or reporting unavailable credentials, consult the relevant entry in the project
profile and the integration guide. Follow the declared credential source and loading method within the user's authorized
task. An absent process variable does not establish that no credential source is configured. Profile and template files
do not load variables. Ask about material missing information without requesting secret values; never display or copy
credentials into the repository. Distinguish configuration/loading problems from network or provider failures when the
available evidence supports that distinction.

## Work at the task's scale

1. Establish the requested outcome and the smallest useful acceptance criteria. A short, clear task description is
   sufficient for a small edit. Use a formal spec when SDD is chosen or the project requires one.
   During requirements/spec analysis, choose verification by observable behavior, risk and existing architecture;
   use `.harness/verification-plan.md` when helpful. Record finite commands and widening triggers at the task's scale.
2. Inspect the affected code and declared project boundaries. Load only matching skills from `.agents/skills/`; do not
   load every skill or start specialist agents by default.
3. Implement the smallest solution that fits the project's current architecture. Avoid speculative abstractions,
   duplicate validation, and unrelated changes.
4. Run the smallest relevant deterministic checks, address failures, and report evidence and limitations. Use semantic
   review for design questions or material uncertainty; it cannot change a tool's result.

## Conditional engineering loop

The primary agent owns the task and its outcome. Consult a specialist only for a concrete unanswered architecture,
domain, security, or other relevant question. Request an independent review when material impact or uncertainty
justifies it; small tasks do not require delegation or another reviewer. Address concrete findings, repeat only affected
checks, and stop when the acceptance criteria are met or a blocker has been reported. Escalate a persistent disagreement
or a product decision to the user. Do not impose a fixed number of iterations.

An optional decision aid may suggest whom to consult. Use it only when installed and explicitly invoked for a bounded
semantic question. Its advice cannot waive a required gate or an escalation required by project instructions.

## Verification contract

Report each applicable check as **passed**, **failed**, **not applicable**, or **unavailable**. Unavailable never means
passed. A failed or unavailable gate declared required by the project blocks completion; report the reason and the
remaining action. Re-run only checks affected by a fix.

- Documentation-only changes do not require a code build or behavior tests unless the project declares a relevant
  documentation check.
- For code changes contained within one local boundary, run focused tests during implementation. Run one project build
  at the end of the complete implementation scope, if the project has a build. Repeat it only after fixing a build
  failure.
- Widen relevant tests and checks when shared code, a shared database, configuration shared across contexts, or module
  boundaries are affected. Honor stricter project-declared gates.
- Use existing analyzers and their configured thresholds. Do not install an overlapping tool or ask an LLM to
  recalculate objective metrics.
- Run existing architecture tests when their protected rule could be affected. Propose a new test only for an explicit,
  deterministic, valuable rule whose violation has meaningful cost and is not already covered. Do not turn a design
  preference into a mechanical ban.
- TDD is optional. When selected, use Red–Green–Refactor; test level and distribution follow the project's architecture
  and test strategy.

## Specs and modular architecture

For any specification-driven development workflow, regardless of tool or skill, name a new spec directory with a
four-digit sequence and slug: `0001-<spec-name>`. Before assigning a number, inspect `.specs/features/`,
`.specs/quick/`, and any additional current or archived spec locations declared in the project profile. Use the next
number after the highest existing prefix; start at `0001` when none exists. The profile may change the default
locations.

For a project organized into bounded contexts, each context owns and validates the configuration controlling its
behavior, persistence, and integrations. Shared infrastructure configuration has an explicit owner. The host composes
configuration sources; ownership does not imply a separate file or database per context. Keep actual secrets in
environment-appropriate sources. Apply technology-specific binding rules only when the project's stack matches them.

Use strategic DDD to reason about context boundaries where applicable. Use tactical DDD and rich domain models only
where invariants justify them; transaction scripts and other simpler patterns remain valid. The project profile declares
its architecture and mandatory gates.

## Reading guidance

Treat `.agentignore` as advisory for routine searches, not an access-control boundary. Read an excluded file when the
task genuinely requires it. Do not skip `AGENTS.md`, `.harness/`, `.agents/skills/`, or `.specs/` because of an ignore
rule.
<!-- ENFATIZA7 HARNESS END -->
