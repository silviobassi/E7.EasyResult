# Project profile

Fill only the sections relevant to this repository. An unfilled field means the agent should inspect existing project
conventions and ask only when a material decision remains unclear. Project-specific instructions take precedence over
harness defaults. The active configuration is `.harness/project-profile.md`; the template is a reference for missing
fields, not a second active profile. Neither file loads configuration or environment variables.

## Project and stack

- Product or repository:
- Languages, frameworks, and supported versions:
- Build system and primary project or solution:

## Tooling decisions (optional, additive)

Preserve filled decisions and native files. Propose only missing fields or an explicitly requested refresh. An existing
profile, including a partial or empty file, is never replaced by the installer. Use
`.harness/templates/project-profile.md` and the tooling catalog to propose relevant additions for review.
Do not re-offer declined capabilities without a relevant change or owner request.

| Capability | Choice: keep/default/custom/decline | Tool / local and candidate versions | Native configuration / origin | Source URL / checked date / verified-unavailable-uncertain | Compatibility: verified-uncertain-unavailable | Reason / refresh trigger |
| --- | --- | --- | --- | --- | --- | --- |
| | | | | | | |

- User-provided tools/references and preferences:
- SDK/JDK/runtime, package manager/wrapper, router and CI inventory:
- Optional sizing review signal, if chosen (200 lines is guidance, not a gate):

## Architecture and boundaries

- Declared architecture and module or bounded-context map:
- Public contracts and allowed dependencies:
- Configuration owner for each bounded context, if applicable:
- Owner of shared infrastructure configuration:
- Locations and validation rules for configuration and secrets:
- Security-sensitive surfaces and trust boundaries, if applicable:

## External integrations and credentials

Record only relevant integrations. Keep secret values outside this profile, the template, commands and logs. A credential
source may be a process environment, a file or a secret manager; describe the existing loading method and its precedence.
Recording a source does not load it or authorize service calls. Reuse equivalent entries elsewhere in an existing profile.
Distinguish configured information, pending decisions and integrations that do not apply; a blank field is unknown.

- Integration and configuration state:
- Required environment variable names:
- Credential source or location (no secret values):
- Loading method and precedence:
- Execution command / working directory / integration guide:
- Authorized use or relevant restrictions:
- Pending decisions or unavailable prerequisites:

## Verification

- Focused test command or discovery method:
- Project build command:
- Wider test and static-analysis commands:
- Existing architecture tests and the rules they protect:
- Existing security checks and the risks they cover, if applicable:
- Required gates beyond the harness defaults:
- Test-level strategy and naming conventions:
- TDD chosen for this work or project, if applicable:

## Verification execution details (optional)

| Check / protected behavior or rule | Command (finite) / cwd / module | When / widening trigger | Timeout or cost budget / origin | Required or optional / failure policy | Evidence status / reason |
| --- | --- | --- | --- | --- | --- |
| | | | | | |

- Native rule identifiers, thresholds and activation evidence:
- Existing warning/error policies and effect of proposed optional rules:
- Test plan location and controls for seeds, clock, environment or external services:
- Commands already covered transitively by lifecycle/tasks (avoid duplicate execution):
- Final build scope and fresh-artifact requirements for reuse:

## Specifications

- Active spec locations: `.specs/features/`, `.specs/quick/` (editable defaults)
- Additional current or archived locations included in numbering:
- Project-specific SDD process, if any:

## Relevant skills and other local rules

- Additional or overriding project skills:
- Other instructions to consult for particular tasks:
