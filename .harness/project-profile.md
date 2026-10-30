# Project profile

Fill only the sections relevant to this repository. An unfilled field means the agent should inspect existing project
conventions and ask only when a material decision remains unclear. Project-specific instructions take precedence over
harness defaults. The active configuration is `.harness/project-profile.md`; the template is a reference for missing
fields, not a second active profile. Neither file loads configuration or environment variables.

## Project and stack

- Product or repository: E7.EasyResult, a .NET result-pattern library.
- Languages, frameworks, and supported versions: C#; library and tests target net10.0. SDK 11 selected; no frontend.
- Build system and primary project or solution: .NET SDK and NuGet; `E7.sln`.

## Tooling decisions (optional, additive)

Preserve filled decisions and native files. Propose only missing fields or an explicitly requested refresh. An existing
profile, including a partial or empty file, is never replaced by the installer. Use
`.harness/templates/project-profile.md` and the tooling catalog to propose relevant additions for review.
Do not re-offer declined capabilities without a relevant change or owner request.

| Capability | Choice: keep/default/custom/decline | Tool / local and candidate versions | Native configuration / origin | Source URL / checked date / verified-unavailable-uncertain | Compatibility: verified-uncertain-unavailable | Reason / refresh trigger |
| --- | --- | --- | --- | --- | --- | --- |
| SDK | custom | .NET SDK 11.0.100-rc.1.26425.128, latestFeature; prereleases allowed | `global.json`; CI installs 10.0.x and the selected 11 RC | https://learn.microsoft.com/en-us/dotnet/core/tools/global-json / 2026-10-30 / verified | local build and tests passed; CI execution unverified | Owner chose SDK 11; .NET 10 retained in CI for net10.0 test runtime. |
| Analysis | keep | SDK Roslyn analyzers | SDK defaults; no repository-specific severity settings found | https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/overview / 2026-10-30 / verified | uncertain: execution and active diagnostics not established | No additional analyzer package or threshold. |
| Formatting | custom | SDK `dotnet format` | `.editorconfig`: C# indentation, braces, using order and file-scoped namespace preference | https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/style-rules/csharp-formatting-options and https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-format / 2026-10-30 / verified | uncertain until format check; SDK 11 RC | Optional check; no build severity or CI gate added. |
| Tests | keep | xUnit 2.9.2, runner 2.8.2, Shouldly 4.3.0, Test SDK 17.12.0 | test project | https://xunit.net/docs/getting-started/v2/getting-started / 2026-10-30 / uncertain | uncertain until test execution | Preserve existing packages. |
| Coverage | keep | coverlet.collector 6.0.2 | test project | https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-code-coverage / 2026-10-30 / verified | uncertain until executed | Optional; no threshold. |
| Manual model advice | decline | off; no catalog | no optional integration installed | guide not installed / 2026-10-30 / unavailable | not applicable | Owner accepted recommendation to leave off. |

- User-provided tools/references and preferences: SDK 11; formatting rules; preserve Jev instructions in AGENTS.md.
- SDK/JDK/runtime, package manager/wrapper, router and CI inventory: local SDKs 10.0.113 and 11.0.100-rc.1.26425.128; NuGet via dotnet; CI uses setup-dotnet for 10.0.x and 11.0.100-rc.1.26425.128; no frontend router.
- Optional sizing review signal, if chosen (200 lines is guidance, not a gate):

## Architecture and boundaries

- Declared architecture and module or bounded-context map: library under `src/E7.EasyResult`, unit tests under `tests/E7.EasyResult.Tests`; no bounded contexts declared.
- Public contracts and allowed dependencies: public Result and railway APIs; tests reference the library.
- Configuration owner for each bounded context, if applicable:
- Owner of shared infrastructure configuration:
- Locations and validation rules for configuration and secrets:
- Security-sensitive surfaces and trust boundaries, if applicable:

## External integrations and credentials

Record only relevant integrations. Keep secret values outside this profile, the template, commands and logs. A credential
source may be a process environment, a file or a secret manager; describe the existing loading method and its precedence.
Recording a source does not load it or authorize service calls. Reuse equivalent entries elsewhere in an existing profile.
Distinguish configured information, pending decisions and integrations that do not apply; a blank field is unknown.

- Jev: agent integration, not a library dependency. `TYPESAFE_API_KEY` source is `~/.config/ai-secrets/jev.env`, as specified in `AGENTS.md`; the file currently contains one unquoted assignment and has mode 0600. For an authorized invocation, use a process-scoped loader that parses this assignment as data without sourcing or executing the file, and passes the value privately in the child environment. If a nonempty process variable already exists, retain it; otherwise use the file. Do not log values or install a global loader. Local loading was checked without a service call; process environments do not persist between commands. Consult `.agents/skills/typesafe-ai/SKILL.md` for invocation guidance; live calls require separate permission and a budget.
- GitHub tagging: workflow passes `GITHUB_TOKEN` from `secrets.GITHUB_TOKEN` to `anothrNick/github-tag-action`; workflow working directory is the repository root.
- NuGet publication: workflow passes `secrets.PUSH_NUGET` directly as `dotnet nuget push --api-key` to nuget.org; no environment variable is declared for it. See `.github/workflows/production.yml`; publication is not authorized by this setup.
- Frontend, database and application HTTP integrations: not applicable to the current library.

## Verification

- Focused test command or discovery method: `dotnet test E7.sln --filter FullyQualifiedName~RailwayExtensionsTests` from repository root; adapt filter to the affected subject.
- Project build command: `dotnet build E7.sln --configuration Release` from repository root, once at end of code scope.
- Wider test and static-analysis commands: `dotnet test E7.sln --configuration Release`; widen for shared behavior. Optional `dotnet test E7.sln --collect:"XPlat Code Coverage"` and `dotnet format whitespace E7.sln --verify-no-changes --no-restore`.
- Existing architecture tests and the rules they protect: none found.
- Existing security checks and the risks they cover, if applicable: none found; assess relevant trust boundaries per task.
- Required gates beyond the harness defaults: CI runs Release tests; no other project-specific gate found.
- Test-level strategy and naming conventions: xUnit facts and Shouldly, descriptive behavior names; cover success, failure, notifications and async where relevant.
- TDD chosen for this work or project, if applicable: not selected.

## Verification execution details (optional)

| Check / protected behavior or rule | Command (finite) / cwd / module | When / widening trigger | Timeout or cost budget / origin | Required or optional / failure policy | Evidence status / reason |
| --- | --- | --- | --- | --- | --- |
| Focused tests | `dotnet test E7.sln --filter FullyQualifiedName~RailwayExtensionsTests`; root | Relevant local behavior; adapt filter | 5 minutes, initial local budget | Required for applicable code change; failure blocks completion | Not applicable at setup; wider suite executed |
| Release build | `dotnet build E7.sln --configuration Release`; root | End of code scope; repeat after build fix | 5 minutes, initial local budget | Required for applicable code change | Passed 2026-10-30 with `--no-restore`, SDK 11 RC, 0 warnings |
| Wider tests | `dotnet test E7.sln --configuration Release`; root | Shared behavior or CI-equivalent verification | 5 minutes, initial local budget | Required when applicable | Passed 2026-10-30 with `--no-build --no-restore`: 30 tests |
| Formatting | `dotnet format whitespace E7.sln --verify-no-changes --no-restore`; root | Optional formatting review | 5 minutes, initial local budget | Optional; reports existing deviations | Passed 2026-10-30 after fixing one existing whitespace deviation |

- Native rule identifiers, thresholds and activation evidence: C# whitespace options use IDE0055; `dotnet format whitespace` detected an existing deviation and passed after correction. File-scoped namespace preference is a suggestion; its activation was not separately tested.
- Existing warning/error policies and effect of proposed optional rules: no TreatWarningsAsErrors or WarningsAsErrors setting found in project files. No new build severity or warning-as-error setting; format verification may report existing differences.
- Test plan location and controls for seeds, clock, environment or external services:
- Commands already covered transitively by lifecycle/tasks (avoid duplicate execution):
- Final build scope and fresh-artifact requirements for reuse:

## Specifications

- Active spec locations: `.specs/features/`, `.specs/quick/` (editable defaults)
- Additional current or archived locations included in numbering:
- Project-specific SDD process, if any:

## Relevant skills and other local rules

- Additional or overriding project skills: `.agents/skills/typesafe-ai/SKILL.md` as directed by AGENTS.md; matching Enfatiza7 skills when relevant.
- Other instructions to consult for particular tasks: `AGENTS.md`, `.harness/verification-plan.md`.
