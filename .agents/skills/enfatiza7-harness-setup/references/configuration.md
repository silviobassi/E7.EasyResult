# Optional native configuration

Inventory first, offer tool/rule by name, then show a reviewed diff for accepted configuration. These samples are not
installed automatically. Preserve native syntax, local thresholds, existing mandatory gates and scopes. Record command,
cwd, severity, activation evidence, version/source/date, choice and effect in the project profile.

## C# / Roslyn

CA1502 (cyclomatic complexity) and CA1506 (type coupling) require support and explicit activation checks for the actual
SDK/analyzer. A passing build does not demonstrate they ran. Inspect SDK, analyzer packages, AnalysisLevel/AnalysisMode,
EnableNETAnalyzers, rule scope, `WarningsAsErrors` and `TreatWarningsAsErrors`. Do not enable all analyzers for two rules.

Example only, after acceptance, for an appropriate `.editorconfig` scope:

```ini
[*.cs]
dotnet_diagnostic.CA1502.severity = warning
dotnet_diagnostic.CA1506.severity = warning
```

Configure thresholds in the analyzer's supported native configuration for its actual version; no universal number.
Confirm activation on representative code or existing analyzer diagnostics. Under warnings-as-errors these warnings
may fail a build: describe that effect before acceptance. Declining optional rules preserves required checks. Do not
disable global warnings-as-errors or suppress unrelated rules to accommodate them. Cognitive complexity is distinct
from cyclomatic complexity; no extra scanner is needed by default. Type coupling does not measure abstraction value.

## ESLint

Reuse the existing flat/legacy config and JS/TS parser. A scope-specific example in an existing compatible flat config
(the threshold comes from an explicit project choice):

```javascript
const projectChosenThreshold = 12; // example, not a harness default
export default [{ files: ['src/**/*.{js,jsx,ts,tsx}'],
  rules: { complexity: ['warn', projectChosenThreshold] } }];
```

Do not replace the project's config with this snippet. A script using `--max-warnings 0` can fail on a warning;
show the exact command/config effect before accepting the rule. Preserve existing Biome/equivalent unless a meaningful
uncovered capability justifies another tool. No `next lint` assumption: inspect actual Next version and package scripts.

## Angular

Use current files as canonical: `package.json`/lockfile, `angular.json`, extended `tsconfig` files, ESLint config,
test runner, scripts and CI. Inspect Nx config/targets only if adopted. Resolve Angular/CLI/Node/TypeScript,
builders and analyzer/runner compatibility before proposing settings. Ask for owner tools/references and record
keep/default/custom/decline, including preserved thresholds/gates. An empty workspace has no activation evidence;
confirm the planned stack and keep version/command/config recommendations provisional.

### Compiler and templates

Inspect each relevant build/test target's effective tsconfig and inheritance. The Angular compiler checks templates
as well as TS; `tsc` or lint alone is not proof. An accepted fragment for an appropriate compatible native config is:

```json
{
  "angularCompilerOptions": {
    "strictTemplates": true
  }
}
```

Merge only the reviewed setting into the existing config; never replace the file. Check effective activation in the
actual compiler version and target. Changing template strictness can introduce compiler errors and block builds;
show this impact before acceptance. Preserve existing strict settings and required gates; address relevant failures
without globally weakening unrelated checks. [Official template checking](https://angular.dev/tools/cli/template-typecheck).

### Angular lint, complexity and formatting

Inspect existing flat/legacy ESLint style and compatible Angular/angular-eslint/TypeScript/ESLint majors and peer
dependencies. Add angular-eslint only for an accepted uncovered capability. Keep a TS scope with its existing parser
and supported inline-template processor, plus an HTML/template scope with the compatible template parser/rules.
Verify representative external and inline templates are actually linted; a TS-only successful lint does not prove this.
Adapt the existing config using that version's documented integration rather than copying an unrelated React config.
Type-checked presets need an explicit value/cost decision; not enabled by default.
[Official angular-eslint integration](https://github.com/angular-eslint/angular-eslint).

The ESLint complexity fragment above can apply to authored Angular TS functions using an accepted local scope and
threshold. It does not measure template branches, cognitive difficulty or abstraction value. Preserve existing
thresholds and explain `--max-warnings`/CI failure behavior: a warning may already be a failing gate. Do not suppress
other warnings globally. Reuse the formatter's check script and TS/HTML support; avoid automatic rewrites or a second
formatter/check for equivalent coverage.

### Accepted architecture rules

Start with declared public APIs, allowed dependency direction and prohibited cycles, not universal folder rules.
Inspect existing import/boundary rules before adding one. In an existing Nx workspace, verify the compatible
`@nx/enforce-module-boundaries`, project tags/constraints and protected workspace-project graph. The rule is not
automatic enforcement of folders inside a single project; do not create libraries or migrate to Nx for it.
Outside Nx, reuse sufficient import lint; offer dependency-cruiser only for a valuable uncovered rule.
Record rule owner/scope/severity and a representative allowed/forbidden case or existing diagnostic demonstrating
activation. Do not duplicate a protected rule; static import cycles do not prove runtime DI/loading correctness.
[Nx rule](https://nx.dev/docs/kb/enforce-module-boundaries), [dependency-cruiser](https://github.com/sverweij/dependency-cruiser).

### Runner and finite commands

Preserve the existing Angular test builder/runner and its native config. Current new CLI workspaces use Vitest;
confirm actual version support and keep adopted Karma/Jasmine/Jest/other tooling without migration. Prefer Angular
TestBed and existing HTTP testing tools for Angular behavior, not React Testing Library or bUnit.
[Official testing](https://angular.dev/guide/testing), [HTTP testing](https://angular.dev/guide/http/testing).

Examples only, after resolving a real project name, local CLI and supported targets/options:

```text
ng test <project> --watch=false
ng lint <project>
ng build <project>
```

Use existing scripts/package-manager conventions with the installed CLI, not a package-downloading invocation.
`ng lint` requires an actual configured lint target; test selectors and flags depend on the builder/version. Inspect
the local CLI help before adapting the [current test command](https://angular.dev/cli/test). Specify project/test scope,
cwd, timeout/cost budget, required/optional status and widening trigger. Inspect scripts/targets already running
tests/lint/build transitively; do not schedule overlapping commands. Focused checks during local work, one final
applicable build, repeated only after fixing its failure; no routine watch/clean/install/full suite. Required
failed/unavailable gates block completion, and optional activation cannot weaken them. Record source URL/date/status,
compatibility and actual passed/failed/not-applicable/unavailable evidence separately.

## Java

Inspect wrapper/JDK, `pom.xml` or Gradle build, rulesets and CI. Reuse PMD/Checkstyle complexity if present. Show plugin
version, rule id/threshold, scope, severity, `failOnViolation`/`ignoreFailures` where supported, lifecycle/task and
failure policy. Verify each property against that plugin/version; do not copy Maven flags into a Gradle configuration.
Existing checks may already run under `verify` or `check`. Discover focused module/test selection in the actual runner,
and run a finite selected lifecycle/task once rather than compiling/testing again via overlapping commands.
Do not add Spring, ArchUnit and Modulith together merely to match another stack. For modular Spring, inspect existing
Modulith structural coverage before proposing ArchUnit rules. SpotBugs/Error Prone/PMD/Checkstyle each need a concrete
missing capability; formatter checks and mutation/coverage are not mandatory per edit.

## References and refresh

Use the official per-tool links and dates in `.harness/tooling-defaults.md`; availability is separate from local
compatibility. When a tool is selected, verify current supported settings for that version and retain owner's sources.
Record unavailable/uncertain sources honestly. Research only selected capabilities at setup or meaningful refresh.
