# Capability-based tooling defaults

These are candidates, not a package installation list. At setup inspect manifests, SDK/JDK and framework versions,
wrappers, lockfiles, scripts, CI, native rule configuration and the project profile. Ask whether the owner wants to
provide tools or references before researching alternatives. Preserve equivalent existing tools and declined choices.
Present each relevant tool by name, capability, reason, cost, compatibility, command/cwd and effect on required gates.
Offer **keep / accept default / custom / decline**. A default is a recommendation, not permission to install packages.

Sources below were accessible on **2026-10-10**. This verifies documentation availability only. Check compatibility
against the consuming project's actual versions before proposing dependencies/configuration. Record source state
`verified`, `unavailable` or `uncertain` separately from tool execution and compatibility. If an existing configuration
cannot be determined, report uncertainty; never infer that a passing build ran an inactive rule.

## .NET backend and Blazor

| Capability | Candidate and official reference | Where / why / condition |
| --- | --- | --- |
| Compilation / static analysis | [SDK Roslyn analyzers](https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/overview) | Existing SDK/build, `.editorconfig` and MSBuild properties first; no npm for pure .NET/Blazor. |
| Optional complexity / coupling | [CA1502](https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/ca1502), [CA1506](https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/ca1506) | Offer individual configuration only when useful; verify support/activation and warning-as-error effects. No universal threshold. |
| Formatting | [dotnet format](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-format) | Existing style configuration; use a verification mode appropriate to the installed SDK when chosen. |
| Behavioral tests | [xUnit](https://xunit.net/docs/getting-started/v3/getting-started) | Candidate for an uncovered runner need; preserve NUnit/MSTest/current xUnit and runner compatibility. |
| HTTP integration | [WebApplicationFactory](https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests) | Observable request/auth/persistence boundary; match ASP.NET Core version. |
| Real resources | [Testcontainers](https://dotnet.testcontainers.org/), [EF testing strategy](https://learn.microsoft.com/en-us/ef/core/testing/choosing-a-testing-strategy) | Only when database/resource semantics matter; account for container runtime, isolation and startup cost. |
| Architecture | [ArchUnitNET](https://github.com/TNG/ArchUnitNET) | Explicit valuable dependency rule; retain existing NetArchTest/equivalent. No duplicate rule. |
| Blazor components | [bUnit](https://bunit.dev/) | Observable component states; real browser tests only for relevant browser/JS behavior. |

## Java backend

Reuse the project's Java architecture/project-rules skills. Match .NET's capabilities, not its package count.

| Capability | Candidate and official reference | Where / why / condition |
| --- | --- | --- |
| Build | [Maven](https://maven.apache.org/guides/), [Gradle](https://docs.gradle.org/current/userguide/userguide.html) | Preserve JDK/toolchain and existing wrapper, modules and lifecycle/tasks. A lifecycle may already run tests/analyzers. |
| Behavioral tests | [JUnit Jupiter](https://docs.junit.org/current/user-guide/) | Use a compatible version; preserve adopted JUnit/TestNG/other runner. No automatic latest-version migration. |
| Spring integration | [Spring Boot testing](https://docs.spring.io/spring-boot/reference/testing/index.html) | Spring only if already adopted; focused slice, MockMvc or full application test according to boundary. |
| Real resources | [Testcontainers Java](https://java.testcontainers.org/) | Persistence/resource semantics require real infrastructure; environment and cost explicit. |
| Architecture | [ArchUnit](https://www.archunit.org/userguide/html/000_Index.html) | Explicit package/layer/module/cycle rule lacking equivalent coverage. |
| Spring module verification | [Spring Modulith](https://docs.spring.io/spring-modulith/reference/verification.html) | Only matching modular Spring projects; structural verification uses ArchUnit, inspect coverage before adding another rule. |
| Defect analysis | [SpotBugs](https://spotbugs.github.io/), [Error Prone](https://errorprone.info/) | Bytecode versus compiler analysis; preserve existing tool. Offer a missing capability, not both by default. |
| Complexity / conventions | [PMD](https://pmd.github.io/pmd/pmd_rules_java_design.html), [Checkstyle](https://checkstyle.org/checks/metrics/cyclomaticcomplexity.html) | Reuse native configuration/thresholds. Avoid overlapping rules; no LLM-calculated metrics. |
| Formatting | [Spotless](https://github.com/diffplug/spotless), [Checkstyle](https://checkstyle.org/) | Spotless integrates formatting; Checkstyle checks conventions. Distinct capabilities do not justify redundant checks. |
| Coverage / mutation | [JaCoCo](https://www.jacoco.org/jacoco/), [PIT](https://pitest.org/) | Optional meaningful evidence for critical code; bounded scope, not every edit, no universal percentage. |

Discover `pom.xml`, `build.gradle`/`build.gradle.kts`, native configs and CI. Show plugin version, phase/task, severity,
`failOnViolation`/`ignoreFailures` and command effect before changes. Resolve wrapper suffix for OS and module selection.
Do not prescribe `clean`, rerun transitive checks or change framework/JDK without a concrete need. Java with Blazor
still needs .NET for that frontend; otherwise no .NET command unless a declared component requires it.

## React and Next.js

| Capability | Candidate and official reference | Where / why / condition |
| --- | --- | --- |
| Types | [TypeScript](https://www.typescriptlang.org/docs/) | Only if adopted/chosen; do not convert a JavaScript project during setup. |
| Lint / complexity | [ESLint complexity](https://eslint.org/docs/latest/rules/complexity) | Preserve ESLint/Biome/equivalent configuration and scripts. Show `--max-warnings` effect before proposing warnings. |
| Behavior | [Vitest](https://vitest.dev/guide/), [React Testing Library](https://testing-library.com/docs/react-testing-library/intro/) | Candidate for React/Next client components; preserve Jest/current runner. |
| API doubles | [MSW](https://mswjs.io/docs/) | Observable frontend behavior where real service semantics are not the subject. |
| Next server boundary | [Next.js testing](https://nextjs.org/docs/app/guides/testing/vitest) | Inspect router/version; current guide warns Vitest does not support async Server Components. Select integration/E2E appropriate to actual server behavior. |
| Dependency boundaries | [dependency-cruiser](https://github.com/sverweij/dependency-cruiser) | Existing import/boundary lint first; add only for a valuable uncovered deterministic rule. |
| Browser flows | [Playwright](https://playwright.dev/docs/intro) | Critical workflow/browser behavior; finite test command, browser installation cost explicit. |
| Accessibility | [Playwright + axe](https://playwright.dev/docs/accessibility-testing) | Relevant interactions; automated findings are partial evidence, not full accessibility compliance. |

## Angular

Inventory `package.json`, lockfiles, `angular.json`, extended `tsconfig` files, lint/runner configuration, scripts and CI;
include `nx.json`/`project.json` only when Nx is adopted. Resolve Angular/CLI, Node, TypeScript, builder and runner
versions from local evidence. Ask for owner tools/references, then offer keep/default/custom/decline per missing
capability. An empty target has a planned stack, not installed tools: confirm it and mark proposals provisional until
source/configuration exists. Accessible sources below were checked on 2026-10-10; local compatibility and execution
remain unverified without the consuming project. No automatic migration, installation or latest-version upgrade.

| Capability | Candidate and official reference | Where / why / condition |
| --- | --- | --- |
| Compilation / template types | [Angular template type checking](https://angular.dev/tools/cli/template-typecheck) | Existing Angular compiler/build checks TS and template bindings; inspect effective `strictTemplates` and config inheritance. A standalone `tsc` or lint pass does not prove template checking. Explain activation/build effects before optional changes. |
| TS and template lint | [angular-eslint](https://github.com/angular-eslint/angular-eslint) with existing ESLint | Offer only for missing Angular coverage; inspect compatible majors, parser and external/inline-template processing. Preserve flat/legacy config and equivalent tools. Type-checked presets have additional cost; not enabled by default. |
| Cyclomatic complexity | [ESLint complexity](https://eslint.org/docs/latest/rules/complexity) | TS-function paths, not template complexity or abstraction quality. Preserve threshold/scope; optional warning can fail under `--max-warnings 0`. No cognitive-complexity scanner by default. |
| Formatting | Existing project formatter | Preserve scripts and supported TS/HTML formatting; propose a named compatible formatter only for an accepted gap. Check mode, not automatic rewrites or another overlapping gate. |
| Unit / component / integration | [Angular testing](https://angular.dev/guide/testing), [TestBed component tests](https://angular.dev/guide/testing/components-basics) | Current new CLI projects use Vitest; verify actual builder/version support. Preserve adopted Karma/Jasmine, Jest or other runners; no migration. Test meaningful logic, inputs/outputs, forms, permissions and UI states. No React Testing Library/bUnit assumption. |
| Service / HTTP boundary | [Angular HTTP testing](https://angular.dev/guide/http/testing) | Existing TestBed/HTTP testing tools and `HttpTestingController` for requests, responses and interceptors. Doubles do not prove real backend authorization, persistence or provider compatibility. |
| Architecture / dependency rules | Existing import lint; [Nx module boundaries](https://nx.dev/docs/kb/enforce-module-boundaries); [dependency-cruiser](https://github.com/sverweij/dependency-cruiser) | Protect declared public APIs, dependency direction or prohibited cycles only when valuable and uncovered. Reuse Nx only when adopted; its rule concerns workspace projects, not arbitrary folders. Otherwise inspect existing lint before considering dependency-cruiser. No Nx migration or duplicate scanner. |
| Browser / accessibility / security | [Playwright](https://playwright.dev/docs/intro), [Playwright + axe](https://playwright.dev/docs/accessibility-testing), [Angular accessibility](https://angular.dev/best-practices/a11y), [Angular security](https://angular.dev/best-practices/security) | Relevant critical flows/trust boundaries only. Automated a11y is partial evidence; route guards do not prove backend authorization. Preserve existing browser tooling and include browser-installation/environment cost in the plan. |
| Build budgets / rendering / performance | [Angular build](https://angular.dev/tools/cli/build), [SSR/hybrid rendering](https://angular.dev/guide/ssr) | Inspect existing budgets and SSR/hydration/PWA adoption. Build budget checks are deterministic for the configured artifact; runtime performance needs controlled conditions. No universal size/time threshold or SSR/PWA adoption. |

Use shared property, contract, mutation and other conditional techniques below only for a stated risk. Select levels
during requirements/specs with the Angular architecture skill and verification guide. Derive finite commands from
scripts/builders; name project/scope, cwd, timeout/cost, required/optional status and widening trigger. Inspect tasks
already run transitively; focused tests during implementation and one final applicable build, repeated for its failure.
Existing required failed/unavailable gates still block completion; semantic review cannot override tool evidence.

## Conditional techniques across stacks

Levels (unit/component/integration/system), techniques (example/property/model/metamorphic/fuzz/combination/mutation)
and objectives (functional/security/accessibility/performance/compatibility) are different axes. Select by requirements.
Pyramid, inverted pyramid, Diamond, Trophy and Honeycomb describe distribution heuristics; no quotas or mandatory TDD.

| Need | Technique / candidates | Cost control |
| --- | --- | --- |
| Invariants over many inputs | Property tests: [FsCheck](https://fscheck.github.io/FsCheck/), [fast-check](https://fast-check.dev/docs/introduction/), existing Java equivalent | Bounded cases and reproducible seed when relevant. |
| Independent deployable consumer/provider | [Pact](https://docs.pact.io/) contract tests | Only actual compatibility risk; no mandatory broker. |
| Critical tests may miss wrong behavior | [Stryker](https://stryker-mutator.io/docs/) / PIT mutation | Selected logic outside routine edit loop. |
| Many configuration combinations | [NIST combinatorial testing](https://www.nist.gov/publications/practical-combinatorial-testing) | Cover meaningful interactions rather than exhaustive combinations. |
| State transitions or absent exact oracle | Model-based / metamorphic tests | Explicit transition model or relation; select stack-compatible implementation only when needed. |
| Untrusted structured input | Fuzzing | Bounded runtime, minimized corpus and reproducible failures; relevant security boundary only. |
| Performance / resilience risk | Load or fault-injection experiment | Controlled environment and budget; timing is variable, no universal per-edit gate. |

Compilers, analyzers and controlled tests provide deterministic evidence within known inputs/config/environment.
Generated/property/fuzz tests need seeds and limits; load tests need controlled conditions. Agent design review and
decision-provider output are semantic, potentially variable advice. They cannot overwrite a tool result or waive a gate.
No NDepend, abstraction indices, mandatory cognitive-complexity scanner or compulsory additional reviewer.

## Freshness and decisions

Academic/engineering background for conditional techniques: [QuickCheck (Claessen/Hughes)](https://research.chalmers.se/publication/?id=155860),
[mutation survey (Jia/Harman)](https://crest.cs.ucl.ac.uk/fileadmin/crest/sebasepaper/JiaH10.pdf),
[model-based taxonomy (Utting/Pretschner/Legeard)](https://mediatum.ub.tum.de/doc/1246357/396788.pdf),
[metamorphic survey (Segura et al.)](https://eprints.whiterose.ac.uk/id/eprint/110335/),
[LLVM libFuzzer](https://llvm.org/docs/LibFuzzer.html), [Testing Trophy](https://kentcdodds.com/blog/the-testing-trophy-and-testing-classifications)
and [Spotify Honeycomb](https://engineering.atspotify.com/2018/1/testing-of-microservices).
These background links are retained from the analysis, not freshly verified by this implementation. Revalidate the
relevant source when selecting its technique; none requires another package or mandatory test distribution.

At setup or meaningful refresh: inventory → owner's references/preferences → relevant official documentation →
compatible nominal proposal → reviewed additive project changes. Record URL, checked date, local/candidate version,
source status and compatibility status separately. Read only selected stack sections. An inaccessible source stays
unavailable/uncertain, never approved by assumption. Refresh on explicit request, changed stack/tooling/version or
evidence of stale guidance. No background monitoring, per-task research or automatic upgrades. Persist declined tools.

Use `.harness/project-profile.md` for local choices and commands. Installer copies guidance only; setup is performed
with the agent or by the owner. Document accepted configurations in native files and equivalent AGENTS/skills/rules as
appropriate to the consuming agent. The common harness is agent-agnostic and does not require a vendor SDK.
