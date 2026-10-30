---
name: enfatiza7-harness-setup
description: Configure or refresh an installed Enfatiza7 harness for actual project stack, tool preferences and finite verification commands. Use at setup or meaningful tooling refresh, not each implementation task.
---

# Harness setup

Read the project profile and only matching sections of `.harness/tooling-defaults.md`. Inspect existing manifests,
versions, wrappers/scripts, CI, native analyzers and architecture tests. Preserve local choices. Missing fields are not
prohibitions. Ask whether the owner has tools or references to provide before researching alternatives.

Present applicable tools by name, capability, why/where, cost and configuration effect. Offer keep, default, custom or
decline. Preserve declined tools and equivalent existing capabilities. Verify relevant official documentation and
version compatibility at setup/meaningful refresh; record source URL/date/status separately from compatibility and
execution evidence. Unavailable never means verified. No automatic latest-version upgrade.

Propose additive profile/native-config changes for review before applying. Preserve filled fields, scripts and required
gates. Check warning-as-error effects before optional activation. Read `references/configuration.md` only when
configuring optional rules. Copying guidance does not install SDK/packages or call services.

Select verification during requirements with `.harness/verification-plan.md` when available. Record finite commands,
cwd, limits, required/optional gates and widening triggers. Small edits need only a short plan. Agent advice cannot
override tool results. No mandatory line ceiling, tool shopping, universal coverage score or repeated full build.

## External integrations and credentials

Inspect relevant existing integrations, their guides and the active `.harness/project-profile.md`. Use
`.harness/templates/project-profile.md`, when available, as a reference for missing fields, not as active configuration.
Preserve filled settings and declined choices. Propose only relevant additions for review; the installer preserves even
empty existing profiles. Reuse equivalent entries in older profiles rather than requiring a particular section title.

Record integration names, required variable names, credential sources, existing loading methods and precedence,
execution command/cwd or guide, and material pending decisions as needed. Keep secret values outside these records.
Distinguish configured information, unresolved fields and integrations that do not apply. Inspect relevant project
evidence before asking the owner about missing information; request locations or methods, never secret values.
If a filled setting appears inconsistent, explain the evidence and propose a correction for review.

A profile entry does not load a credential or authorize a service call. Check the actual command's loading behavior;
an absent process variable may still have a configured source. Reuse the declared loading method without executing an
arbitrary credential file as shell code. If no supported method exists, record it as pending. Configuration discovery
does not prove service availability; service validation requires authorization for that call. Report configuration,
loading, network or provider problems only as supported by the evidence. Do not add a global credential loader.

## Optional manual model advice

At initial setup or an explicitly requested refresh, offer the owner the optional
local model catalog and `off`/`advisory` modes. Preserve an existing manual choice,
catalog, mode, and refusal; do not re-offer a declined capability during each
implementation. An empty catalog is valid for later registration. If declined,
leave this capability absent, or retain an existing `off` selection without
deleting its profiles. Reactivation requires an explicit owner choice.

Use the [model advice guide](../../../.harness/integrations/model-advice.md) when
the capability is installed; otherwise consult the distributor's guide before
proposing installation. Ask for owner-supplied profiles rather than researching
model rankings or providers on each task. Present the mode, profile IDs, provider,
model, description, and optional effort before configuration changes. Declared
effort/capabilities are local values, not verified platform support or measured
quality/cost. Keep sensitive data outside descriptions; structural validation
cannot detect every secret. Validate the proposed catalog/configuration locally.

For a new selection, propose the installer's explicit `--model-advice advisory`
and optional `--model-catalog` preview; an empty catalog can be populated later.
For an existing selection, preserve profiles and edit only what the owner requests;
omitted flags preserve configuration bytes. Configuring a catalog invokes no
service, installs no SDK, and neither selects nor reconfigures a decision provider.
Normal advice remains local and manual. External model consultation requires a
separate explicit request and valid advisory configuration with profiles. Model
choice never changes required gates or authorizes execution/spawn automatically.
