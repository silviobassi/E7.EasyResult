# Choose verification during requirements

Before implementation, the owner alone or with the agent decides which observable outcomes, failure paths and boundaries
need evidence. Reuse the declared architecture and existing tests/analyzers, consulting only relevant architecture skills
and the tooling catalog. A spec can hold this plan; a small clear edit needs only a sentence. No particular SDD tool,
test distribution, TDD workflow, complete form or extra reviewer is mandatory.

For each meaningful check, record what it protects, why it matters, where the boundary is and how/when it runs:

| Behavior / acceptance / risk | Level and technique | Tool / command / cwd | Real resource or double; controls | Required/optional / when / budget | Widen or rerun trigger |
| --- | --- | --- | --- | --- | --- |
| | | | | | |

Choose levels (unit/component/integration/system), techniques (examples/properties/model/metamorphic/fuzz/combination/
mutation) and objectives separately. Distribution shapes such as Pyramid, inverted Pyramid, Diamond, Trophy and
Honeycomb are heuristics, not mandatory ratios. If the need is already covered, reuse that evidence instead of adding
another tool/test. Name candidates only for missing capabilities; persist keep/default/custom/decline in the profile.

## Architecture and behavior examples

| Architecture / risk | Useful evidence | Conditional stack candidates |
| --- | --- | --- |
| Transaction Script / Vertical Slice local use case | Inputs, observable outcome, meaningful errors at use-case boundary | Existing unit/integration runner; no E2E without browser/system risk. |
| Domain model with invariants | Behavior and invariants; property tests when input space warrants | Existing runner; FsCheck/fast-check/Java equivalent only if useful. |
| Active Record / persistence semantics | Real database behavior for mappings/queries/transactions | Existing integration fixture; Testcontainers only if needed and available. |
| CQRS | Command/query contracts; replay/idempotence only if requirements use them | Existing runner/contract checks; no event system introduced by testing. |
| Modular monolith | Explicit prohibited dependency or public-contract rule with meaningful violation cost | Existing NetArchTest/ArchUnitNET/ArchUnit; Spring Modulith only matching project. No duplicate rule. |
| React / Next / Blazor UI | Observable states, permissions/forms; critical browser flow only when relevant | Existing components runner/RTL for React or bUnit for Blazor; Playwright conditional. Async RSC needs supported server/integration level. |
| Angular logic / components | Observable logic, inputs/outputs, DOM, forms and loading/error/permission states | Existing compatible runner and Angular TestBed/component fixtures; plain unit tests for logic without Angular dependencies. Preserve runner and standalone/NgModule conventions; no RTL/bUnit assumption. |
| Angular service / HTTP / feature integration | Meaningful requests/responses/errors, interceptors, navigation and composed feature behavior | Existing Angular HTTP testing tools/HttpTestingController and integration fixtures. Doubles do not prove real backend authorization, persistence or consumer/provider compatibility. |
| Angular architectural boundary | Declared public API, allowed dependency direction or prohibited cycle with meaningful cost | Existing import/boundary lint first; reuse @nx/enforce-module-boundaries only in adopted Nx workspaces (project relations, not arbitrary folders). Conditional dependency-cruiser for an uncovered rule; no migration or duplicate scanner. |
| Angular browser / adopted SSR, hydration or PWA | Critical real-browser/server behavior not covered by component/HTTP tests | Existing E2E tools, Playwright only for an accepted gap; controlled environment and finite scope/cost. Browser suite, rendering features and offline behavior are not mandatory. |
| Independent consumer/provider releases | Contract compatibility risk | Existing contract tests; Pact if actual uncovered risk. |
| Shared configuration/auth boundary | Ownership, validation and allowed/denied behavior across affected contexts | Widen existing integration/security/architecture checks as applicable. |

For Angular, inspect current code, Angular/CLI/TS versions, scripts/builders and native configuration as canonical.
Use the matching Angular skill and catalog section. Current new CLI projects use Vitest; preserve adopted
Karma/Jasmine/Jest/other compatible runners rather than migrating. Distinguish effective compiler/template checks,
TS/external/inline-template lint and TS-function complexity; do not substitute a passing lint or plain tsc for template
checking. Thresholds, type-checked presets, formatting, coverage/mutation and existing build budgets remain local
choices; accessibility/security/performance checks protect relevant risks and carry their evidence limits. Empty
projects need owner-confirmed provisional proposals, not invented versions or executable commands.

Architecture checks require an explicit, deterministic, valuable rule not already protected. A preference such as method
size or quantity of interfaces is not a mechanical ban. Existing analyzers calculate metrics with native thresholds;
agent review interprets design and cannot recalculate/override tool results. A cohesive file exceeding the optional
200-line signal does not fail acceptance. Cognitive complexity is a different concern, not another mandatory scanner.

## Finite execution

Use actual scripts, wrapper/module tasks and native configs; do not invent commands. Prefer finite test-run mode over
watch mode. Record a project-derived timeout or cost budget and stop/report unavailable on environment/tool failure.
Documentation needs relevant docs checks only. Local code uses focused tests during implementation; one project build
at the end of the complete scope when one exists. Widen tests for affected shared code, databases/config/contracts or
module boundaries, honoring stricter project gates. A Maven/Gradle lifecycle or package script may already perform
tests/analysis; inspect its dependency graph before scheduling another execution. No routine `clean`, SDK restore,
mutation run, browser suite or complete scan unless the change/required gate warrants it.

After a fix rerun only affected checks; repeat build when fixing its failure. Reuse evidence only while relevant code,
configuration, environment and required freshness remain valid. `--no-build` needs proven fresh artifacts. No fixed
iteration count: stop when acceptance and required gates are met, or report the blocker. Extra checks need a concrete
unresolved risk or changed input. Optional decision-provider unavailability falls back to ordinary agent reasoning;
required gate failure/unavailability still blocks completion.

Report **passed / failed / not applicable / unavailable** with command, relevant scope and reason. Controlled tool
results are evidence; property/fuzz/load experiments need relevant seeds/time/environment controls. Semantic agent
review is advisory and variable. Automated accessibility checks are partial evidence. No universal coverage percentage,
tool installation, model confidence threshold or specialist loop.
